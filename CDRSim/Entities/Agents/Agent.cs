using CDRSim.Simulation;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

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


        Random random = new Random();

        public abstract void Initialize(List<Agent> agents);
        public abstract void UpdateActivityInterval();
        public abstract Call InitiateCall(int currentTime);
        public virtual Call MakeCall(int currentTime, int length)
        {
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            Agent agentToCall = null;
            double agentToCallTie = 0;
            var randomValue = random.NextDouble();
            foreach (var contact in Contacts)
            {
                if (randomValue < contact.Value)
                {
                    if (!contact.Key.Busy)
                    {
                        agentToCall = contact.Key;
                        agentToCallTie = contact.Value;
                    }
                    break;
                }
            }
            if (agentToCall != null)
            {
                if (this is Talker && agentToCall is Talker)
                    length *= 3;
                if (Information.Mode == SimulationMode.Information)
                {
                    if (this.Aware)
                    {
                        var transferProbability = GetInfoTransferProbability(currentTime, agentToCallTie);
                        randomValue = random.NextDouble();
                        if (randomValue < transferProbability)
                        {
                            agentToCall.Aware = true;
                            length += Information.GetInfoTransferTime();
                        }
                    }
                }
                var call = new Call(this, agentToCall, currentTime, length);
                _callEndTime = currentTime + length;
                Busy = true;
                agentToCall.Busy = true;
                UpdateActivityInterval();
                _activateTime = _callEndTime + ActivityInterval;
                return call;
            }
            return null;
        }
 
        public virtual Call Check(int currentTime)
        {
            if (currentTime >= _callEndTime)
                Busy = false;
            if (currentTime >= _activateTime)
                return InitiateCall(currentTime);
            else
                return null;
        }

        public virtual double GetInfoTransferProbability(int currentTime, double agentsConnection)
        {
            var relativeAgentImportance = agentsConnection / Contacts.Max(a => a.Value);
            //var result = Information.GetRevenance(currentTime) + Information.Importance + InterestDegree + relativeAgentImportance;
            //result /= 4;

            var result = Information.GetRevenance(currentTime) + Information.Importance * 0.045 + InterestDegree * 0.02 + relativeAgentImportance * 0.02;

            //using (StreamWriter file = new StreamWriter("prob.txt", true))
            //{
            //    file.WriteLine(result);
            //}

            //if (currentTime < 1000) {
            //    Console.WriteLine(Information.Importance);
            //    Console.WriteLine(InterestDegree);
            //    Console.WriteLine(relativeAgentImportance);
            //}
            return result;
        }
    }
}
