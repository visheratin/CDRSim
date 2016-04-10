﻿using CDRSim.Simulation;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Concurrent;
using CDRSim.Helpers;
using CDRSim.Parameters;
using CDRSim.Experiments;
using MathNet.Numerics.Distributions;

namespace CDRSim.Entities.Agents
{
    public abstract class Agent
    {
        public int Id { get; set; }
        public AgentType Type { get; set; }
        public Agent[] Contacts { get; set; }
        public double[] ContactProbabilities { get; set; }
        public double[] RealContactProbabilities { get; set; }
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
        public AgentConfigurator Config { get; set; }
        private Random random;

        public abstract void Initialize(IEnumerable<Agent> agents);

        public virtual void CreateInitContacts(IEnumerable<Agent> agents, AgentType type)
        {
            _fillIndex = 0;
            var agentIndex = Id;

            Config = new AgentConfigurator(type);
            var strongConnectionsNumber = 0;
            var contactsNumber = 0;
            Config.SetContactsConfig(ref strongConnectionsNumber, ref contactsNumber);
            if (contactsNumber > ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber - 1)
                contactsNumber = ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber - 1;
            if (strongConnectionsNumber > contactsNumber)
                strongConnectionsNumber = contactsNumber;
            StrongContactsCount = strongConnectionsNumber;
            ContactsCount = contactsNumber;
            Contacts = new Agent[contactsNumber];
            ContactProbabilities = new double[contactsNumber];
            RealContactProbabilities = new double[contactsNumber];
            CalledContacts = new List<Agent>();

            random = new Random(Id * Id + ContactsCount * StrongContactsCount);
            var k = (int)ExperimentGlobal.Instance.Parameters.Agents.First(a => a.Type == type).ContactsMean;

            k /= 2;
            var rewireProbability = 0.7;
            for (int i = agentIndex - k; i <= agentIndex + k; i++)
            {
                if (_fillIndex == ContactsCount)
                    break;
                if (i == agentIndex || i < 0 || i > agents.Count() - 1)
                    continue;
                var res = random.NextDouble();
                Agent currentAgent;
                if (res < rewireProbability)
                    currentAgent = agents.ElementAt(i);
                else
                {
                    var newIndex = random.Next(agents.Count() - 1);
                    currentAgent = agents.ElementAt(newIndex);
                    while(currentAgent == this)
                    {
                        newIndex = random.Next(agents.Count() - 1);
                        currentAgent = agents.ElementAt(newIndex);
                    }
                }

                Contacts[_fillIndex] = currentAgent;
                if (random.NextDouble() > ExperimentGlobal.Instance.ContactProbability &&
                    currentAgent._fillIndex < (currentAgent.ContactsCount - 1) && currentAgent.Contacts != null &&
                    !currentAgent.Contacts.Contains(this))
                {
                    currentAgent.Contacts[currentAgent._fillIndex] = this;
                    currentAgent._fillIndex++;
                }
                _fillIndex++;
            }
        }

        public void CreateRestContacts(IEnumerable<Agent> agents, AgentType type)
        {
            var rewireProbability = 0.3;
            while (_fillIndex < ContactsCount)
            {
                var added = false;
                var res = random.NextDouble();
                {
                    if (res > rewireProbability)
                    {
                        var newContact = GetNewContact();
                        if (newContact != this && !Contacts.Contains(newContact))
                        {
                            Contacts[_fillIndex] = newContact;
                            if (random.NextDouble() > ExperimentGlobal.Instance.ContactProbability &&
                                newContact._fillIndex < (newContact.ContactsCount - 1) &&
                                !newContact.Contacts.Contains(this))
                            {
                                newContact.Contacts[newContact._fillIndex] = this;
                                newContact._fillIndex++;
                            }
                            added = true;
                        }
                    }
                    else
                    {
                        var newIndex = random.Next(agents.Count() - 1);
                        var newContact = agents.ElementAt(newIndex);
                        if (newContact != this && !Contacts.Contains(newContact))
                        {
                            Contacts[_fillIndex] = newContact;
                            if (random.NextDouble() > ExperimentGlobal.Instance.ContactProbability &&
                                newContact._fillIndex < (newContact.ContactsCount - 1) &&
                                !newContact.Contacts.Contains(this))
                            {
                                newContact.Contacts[newContact._fillIndex] = this;
                                newContact._fillIndex++;
                            }
                            added = true;
                        }
                    }
                }
                if (added)
                    _fillIndex++;
            }
        }

        private void ShuffleContacts()
        {
            for (int i = 0; i < Contacts.Length; i++)
            {
                var swapIndex = random.Next(Contacts.Length - 1);
                var t = Contacts[i];
                Contacts[i] = Contacts[swapIndex];
                Contacts[swapIndex] = t;
            }
        }

        public void SetProbabilities(double strongProbabilyFraction, double strongConnectionsIntervalPercent)
        {
            //ShuffleContacts();
            if (StrongContactsCount == ContactsCount)
                strongProbabilyFraction = 1;
            var strongConnectionsInterval = strongProbabilyFraction / StrongContactsCount;
            var strongConnectionsIntervalMin = 0.85 * strongConnectionsInterval;
            var strongConnectionsIntervalDiff = strongConnectionsInterval * 1.15 - strongConnectionsIntervalMin;
            double probabilitySum = 0;
            for (int i = 0; i < Contacts.Length; i++)
            {
                var probability = 0.0;
                if (i < StrongContactsCount - 1)
                {
                    probability = strongConnectionsIntervalMin + random.NextDouble() * strongConnectionsIntervalDiff;
                }
                else if (i == StrongContactsCount - 1)
                {
                    probability = Math.Abs(strongProbabilyFraction - probabilitySum);
                }
                else
                {
                    probability = (1 - probabilitySum) / (Contacts.Length - i);
                }
                if (probability < 0)
                {
                    Console.WriteLine(probability);
                }
                probabilitySum += probability;
                ContactProbabilities[i] = probabilitySum;
                RealContactProbabilities[i] = probability;
            }
        }

        private Agent GetNewContact()
        {
            var contactIndex = random.Next(_fillIndex - 1);
            var itemIndex = random.Next(Contacts[contactIndex].Contacts.Length - 1);
            return Contacts[contactIndex].Contacts[itemIndex];
        }

        public virtual void Create(IEnumerable<Agent> agents, AgentType type,
            double strongProbabilyFraction, double strongConnectionsIntervalPercent)
        {
            ActivityInterval = Config.SetActivityInterval();
            _activateTime = ActivityInterval;

            CreateRestContacts(agents, type);
            SetProbabilities(strongProbabilyFraction, strongConnectionsIntervalPercent);
            if (ExperimentGlobal.Instance.Parameters.Simulation.IsCritical)
            {
                InterestDegree = ExperimentGlobal.Instance.Parameters.Information.InterestDegree +
                    (1 - ExperimentGlobal.Instance.Parameters.Information.InterestDegree) * random.NextDouble();
                //var distribution = new Normal(1, 1);
                //var number = -1.0;
                //number = distribution.Sample();
                //if (number < 0)
                //    number = 0;
                //if (number > 1)
                //    number = 1;
                //while (number < 0 || number > 1)
                //{
                //    number = distribution.Sample();
                //    if (number < 0)
                //        number = 0;
                //}
                //Console.WriteLine(number);
                //InterestDegree = number;
                //switch (type)
                //{
                //    case AgentType.Regular:
                //        InterestDegree = 0.6 + 0.4 * random.NextDouble();
                //        break;
                //    case AgentType.Talker:
                //        InterestDegree = 0.8 + 0.2 * random.NextDouble();
                //        break;
                //    case AgentType.Organizer:
                //        InterestDegree = 1;
                //        break;
                //    default:
                //        break;
                //}
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
                        var calltransfer = false;
                        if (ExperimentGlobal.Instance.Parameters.Simulation.SimulationMode == SimulationMode.Information && Aware)
                        {
                            var transferProbability = GetInfoTransferProbability(currentTime, i);
                            var randomValue = random.NextDouble();
                            if (randomValue < transferProbability)
                            {
                                //Contacts[i].Aware = true;
                                if(!Contacts[i].Aware)
                                    calltransfer = true;
                                length += Information.GetInfoTransferTime();
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
            Agent agentToCall = null;
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
                var calltransfer = false;
                if (this is Talker && agentToCall is Talker)
                    length *= 3;
                    if (ExperimentGlobal.Instance.Parameters.Simulation.SimulationMode == SimulationMode.Information)
                {
                        if (Aware)
                    {
                            var transferProbability = GetInfoTransferProbability(currentTime, index);
                        randomValue = random.NextDouble();
                        if (randomValue < transferProbability)
                        {
                                //agentToCall.Aware = true;
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
                if (CurrentCall.Transfer == 1)
                    CurrentCall.To.Aware = true;
                CurrentCall = null;
            }
            if (!Busy && currentTime >= _activateTime)
            {
                return InitiateCall(currentTime);
            }
            else
                return null;
        }

        public virtual double GetInfoTransferProbability(int currentTime, int index)
        {
            var relativeAgentImportance = RealContactProbabilities[index] / RealContactProbabilities.Max();
            //var result = Information.GetRevenance(currentTime) + Information.Importance + InterestDegree + relativeAgentImportance;
            //result /= 4;

            var result = Information.GetRelevance(currentTime) + 
                ExperimentGlobal.Instance.Parameters.Information.Importance + InterestDegree + relativeAgentImportance;

           //result = Math.Sqrt(result);

            result /= 3;

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
