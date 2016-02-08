using CDRSim.Simulation;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Concurrent;
using CDRSim.Helpers;
using CDRSim.Parameters;

namespace CDRSim.Entities.Agents
{
    public abstract class Agent
    {
        public int Id { get; set; }
        public Dictionary<Agent, double> Contacts { get; set; }
        public int ActivityInterval { get; set; }
        public int _activateTime { get; set; }
        public int _callEndTime { get; set; }
        public double InterestDegree { get; set; }
        public bool Busy { get; set; }
        public bool Aware { get; set; }
        public Call CurrentCall { get; set; }

        public abstract void Initialize(BlockingCollection<Agent> agents);

        public virtual void Create(BlockingCollection<Agent> agents, AgentType type,
            double strongProbabilyFraction, double strongConnectionsIntervalPercent)
        {
            var agconfig = new AgentConfigurator(type);
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            ActivityInterval = agconfig.SetActivityInterval();
            _activateTime = ActivityInterval;

            var strongConnectionsNumber = 0;
            var contactsNumber = 0;

            agconfig.SetContactsConfig(ref strongConnectionsNumber, ref contactsNumber);

            var contactsLeft = contactsNumber;
            var strongConnectionsInterval = strongProbabilyFraction / strongConnectionsNumber;
            var strongConnectionsIntervalMin = strongConnectionsIntervalPercent * strongConnectionsInterval;
            var strongConnectionsIntervalDiff = strongConnectionsInterval - strongConnectionsIntervalMin;

            var usedAgents = new List<Agent>();

            double probabilitySum = 0;

            var contactAgents = agents.Where(a => a.Contacts.Keys.Contains(this) && !Contacts.Keys.Contains(a)).ToList();
            for (int i = 0; i < contactsNumber; i++)
            {
                Agent currentAgent = null;
                var getContact = random.NextDouble();
                if (getContact > 0.3 && contactAgents.Count > 0)
                {
                    currentAgent = contactAgents[random.Next(0, contactAgents.Count() - 1)];
                }
                else
                {
                    currentAgent = agents.ElementAt(random.Next(agents.Count - 1));
                }
                if (usedAgents.Contains(currentAgent))
                {
                    continue;
                }
                usedAgents.Add(currentAgent);
                var probability = 0.0;

                if (i < strongConnectionsNumber)
                {
                    probability = strongConnectionsIntervalMin + random.NextDouble() * strongConnectionsIntervalDiff;
                }
                else
                {
                    probability = (1 - probabilitySum) / contactsLeft;
                }

                probabilitySum += probability;
                Contacts.Add(currentAgent, probabilitySum);
                contactsLeft--;
            }
        }
        public abstract void UpdateActivityInterval();
        public abstract Call InitiateCall(int currentTime);
        public virtual Call MakeCall(int currentTime, int length)
        {
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            Agent agentToCall = null;
            double agentToCallTie = 0;
            var randomValue = random.NextDouble();
            var contact = Contacts.FirstOrDefault(c => c.Value >= randomValue);
            if (contact.Key != null && !contact.Key.Busy)
            {
                agentToCall = contact.Key;
                agentToCallTie = contact.Value;
                var calltransfer = false;
                if (this is Talker && agentToCall is Talker)
                    length *= 3;
                if (Information.Mode == SimulationMode.Information)
                {
                    if (this.Aware)
                    {
                        var transferProbability = GetInfoTransferProbability(currentTime, agentToCallTie);
                        randomValue = random.NextDouble();
                        if (transferProbability > 0.5)
                        {
                            agentToCall.Aware = true;
                            calltransfer = true;
                            length += Information.GetInfoTransferTime();
                        }
                    }
                }
                var call = new Call(this, agentToCall, currentTime, length);
                _callEndTime = currentTime + length;
                agentToCall._callEndTime = currentTime + length;
                Busy = true;
                agentToCall.Busy = true;
                UpdateActivityInterval();
                _activateTime = _callEndTime + ActivityInterval;
                if (calltransfer)
                    call.Transfer = 1;
                CurrentCall = call;
                return call;
            }
            return null;
        }
 
        public virtual Call Check(int currentTime)
        {
            if (CurrentCall != null && currentTime >= _callEndTime)
            {
                CurrentCall.From.Busy = false;
                CurrentCall.To.Busy = false;
                CurrentCall = null;
            }
            if (!Busy && currentTime >= _activateTime)
            {
                return InitiateCall(currentTime);
            }
            else
                return null;
        }

        public virtual double GetInfoTransferProbability(int currentTime, double agentsConnection)
        {
            var relativeAgentImportance = agentsConnection / Contacts.Max(a => a.Value);
            var result = Information.GetRevenance(currentTime) + Information.Importance + InterestDegree + relativeAgentImportance;
            result /= 4;
            //Console.WriteLine(result);
            return result;
        }
    }
}
