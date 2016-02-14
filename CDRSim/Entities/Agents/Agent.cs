using CDRSim.Simulation;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Concurrent;
using CDRSim.Helpers;
using CDRSim.Parameters;
using CDRSim.Experiments;

namespace CDRSim.Entities.Agents
{
    public abstract class Agent
    {
        public int Id { get; set; }
        public AgentType Type { get; set; }
        public Agent[] Contacts { get; set; }
        public double[] ContactProbabilities { get; set; }
        public int ContactsCount { get; set; }
        public int StrongContactsCount { get; set; }
        public int ActivityInterval { get; set; }
        public int _activateTime { get; set; }
        public int _callEndTime { get; set; }
        public double InterestDegree { get; set; }
        public bool Busy { get; set; }
        public bool Aware { get; set; }
        public Call CurrentCall { get; set; }
        private int _fillIndex { get; set; }
        private List<Agent> CalledContacts { get; set; }

        public abstract void Initialize(IEnumerable<Agent> agents);

        public virtual void CreateInitContacts(IEnumerable<Agent> agents, AgentType type)
        {
            _fillIndex = 0;
            var agentIndex = Id;

            var agconfig = new AgentConfigurator(AgentType.Organizer);
            var strongConnectionsNumber = 0;
            var contactsNumber = 0;
            agconfig.SetContactsConfig(ref strongConnectionsNumber, ref contactsNumber);
            StrongContactsCount = strongConnectionsNumber;
            ContactsCount = contactsNumber;
            Contacts = new Agent[contactsNumber];
            ContactProbabilities = new double[contactsNumber];
            CalledContacts = new List<Agent>();

            for (int i = agentIndex - 2; i < agentIndex + 3; i++)
            {
                if (_fillIndex == ContactsCount)
                    break;
                if (i == agentIndex || i < 0 || i > agents.Count() - 1)
                    continue;
                var currentAgent = agents.ElementAt(i);
                
                Contacts[_fillIndex] = currentAgent;
                var random = new Random();
                if(random.NextDouble() > ExperimentGlobal.Instance.ContactProbability && 
                    currentAgent._fillIndex < (currentAgent.ContactsCount - 1) && currentAgent.Contacts != null &&
                    !currentAgent.Contacts.Contains(this))
                {
                    currentAgent.Contacts[currentAgent._fillIndex] = this;
                }
                _fillIndex++;
            }
        }

        public void CreateRestContacts(IEnumerable<Agent> agents, AgentType type)
        {
            var random = new Random(DateTime.Now.Millisecond + Id * Id);
            var rewireProbability = 0.5;
            while (_fillIndex < ContactsCount)
            {
                var added = false;
                var res = random.NextDouble();
                {
                    res = random.NextDouble();
                    if (res > rewireProbability)
                    {
                        var newContact = GetNewContact();
                        if (!Contacts.Contains(newContact))
                        {
                            Contacts[_fillIndex] = newContact;
                            if (random.NextDouble() > ExperimentGlobal.Instance.ContactProbability &&
                                newContact._fillIndex < (newContact.ContactsCount - 1) &&
                                !newContact.Contacts.Contains(this))
                            {
                                newContact.Contacts[newContact._fillIndex] = this;
                            }
                            added = true;
                        }
                    }
                    else
                    {
                        var newContact = agents.ElementAt(random.Next(agents.Count() - 1));
                        if (!Contacts.Contains(newContact))
                        {
                            Contacts[_fillIndex] = newContact;
                            if (random.NextDouble() > ExperimentGlobal.Instance.ContactProbability &&
                                newContact._fillIndex < (newContact.ContactsCount - 1) &&
                                !newContact.Contacts.Contains(this))
                            {
                                newContact.Contacts[newContact._fillIndex] = this;
                            }
                            added = true;
                        }
                    }
                }
                if(added)
                    _fillIndex++;
            }
        }

        public void SetProbabilities(double strongProbabilyFraction, double strongConnectionsIntervalPercent)
        {
            var strongConnectionsInterval = strongProbabilyFraction / StrongContactsCount;
            var strongConnectionsIntervalMin = strongConnectionsIntervalPercent * strongConnectionsInterval;
            var strongConnectionsIntervalDiff = strongConnectionsInterval - strongConnectionsIntervalMin;
            var random = new Random();
            double probabilitySum = 0;
            for (int i = 0; i < Contacts.Length; i++)
            {
                var probability = 0.0;
                if (i < StrongContactsCount)
                {
                    probability = strongConnectionsIntervalMin + random.NextDouble() * strongConnectionsIntervalDiff;
                }
                else
                {
                    probability = (1 - probabilitySum) / (Contacts.Length - i);
                }
                ContactProbabilities[i] = probability;
                probabilitySum += probability;
            }
        }

        private Agent GetNewContact()
        {
            var random = new Random();
            var contactIndex = random.Next(_fillIndex - 1);
            var itemIndex = random.Next(Contacts[contactIndex].Contacts.Length - 1);
            return Contacts[contactIndex].Contacts[itemIndex];
        }

        public virtual void Create(IEnumerable<Agent> agents, AgentType type,
            double strongProbabilyFraction, double strongConnectionsIntervalPercent)
        {
            var agconfig = new AgentConfigurator(type);
            ActivityInterval = agconfig.SetActivityInterval();
            _activateTime = ActivityInterval;

            CreateRestContacts(agents, type);
            SetProbabilities(strongProbabilyFraction, strongConnectionsIntervalPercent);
            var random = new Random();
            switch (type)
            {
                case AgentType.Regular:
                    InterestDegree = 0.6 + 0.4 * random.NextDouble();
                    break;
                case AgentType.Talker:
                    InterestDegree = 0.8 + 0.2 * random.NextDouble();
                    break;
                case AgentType.Organizer:
                    InterestDegree = 1;
                    break;
                default:
                    break;
            }
        }
        public abstract void UpdateActivityInterval();
        public abstract Call InitiateCall(int currentTime);
        public virtual Call MakeCall(int currentTime, int length)
        {
            if (ExperimentGlobal.Instance.Parameters.Simulation.IsCritical)
            {
                for (int i = 0; i < Contacts.Length; i++)
                {
                    if (!CalledContacts.Contains(Contacts[i]) && !Contacts[i].Busy)
                    {
                        var random = new Random((int)DateTime.Now.ToBinary() + Id);
                        var calltransfer = false;
                        if (Information.Mode == SimulationMode.Information)
                        {
                            if (Aware && !Contacts[i].Aware)
                            {
                                var transferProbability = GetInfoTransferProbability(currentTime, ContactProbabilities[i]);
                                var randomValue = random.NextDouble();
                                if (randomValue < transferProbability)
                                {
                                    Contacts[i].Aware = true;
                                    calltransfer = true;
                                    length += Information.GetInfoTransferTime();
                                }
                            }
                        }
                        var call = new Call(this, Contacts[i], currentTime, length);
                        _callEndTime = currentTime + length;
                        Contacts[i]._callEndTime = currentTime + length;
                        Busy = true;
                        Contacts[i].Busy = true;
                        UpdateActivityInterval();
                        _activateTime = _callEndTime + ActivityInterval;
                        if (calltransfer)
                            call.Transfer = 1;
                        CurrentCall = call;
                        CalledContacts.Add(Contacts[i]);
                        return call;
                    }
                }
                return null;
            }
            else
            {
                var random = new Random((int)DateTime.Now.ToBinary() + Id);
                Agent agentToCall = null;
                double agentToCallTie = 0;
                var randomValue = random.NextDouble();
                var index = -1;
                for (int i = 0; i < ContactProbabilities.Length; i++)
                {
                    if (ContactProbabilities[i] >= randomValue)
                    {
                        index = i;
                        break;
                    }
                }
                if (index >= 0 && !Contacts[index].Busy)
                {
                    var contact = Contacts[index];
                    agentToCall = contact;
                    agentToCallTie = ContactProbabilities[index];
                    var calltransfer = false;
                    if (this is Talker && agentToCall is Talker)
                        length *= 3;
                    if (Information.Mode == SimulationMode.Information)
                    {
                        if (Aware && !agentToCall.Aware)
                        {
                            var transferProbability = GetInfoTransferProbability(currentTime, agentToCallTie);
                            randomValue = random.NextDouble();
                            if (randomValue < transferProbability)
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
            var relativeAgentImportance = agentsConnection / ContactProbabilities.Max();
            var result = Information.GetRevenance(currentTime) + Information.Importance + InterestDegree + relativeAgentImportance;
            result /= 4;
            //Console.WriteLine(result);
            return result;
        }
    }
}
