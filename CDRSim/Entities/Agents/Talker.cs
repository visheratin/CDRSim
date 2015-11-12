using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using System.Configuration;
using System.Collections.Specialized;
using CDRSim.Helpers;

namespace CDRSim.Entities.Agents
{
    public class Talker : Agent
    {
        public Talker(int id)
        {
            Id = id;
            Contacts = new Dictionary<Agent, double>();
        }

        public override void Initialize(List<Agent> agents)
        {
            var agconfig = new AgentConfigurator("Talker");
            ActivityInterval = agconfig.SetActivityInterval();
            _activateTime = ActivityInterval;

            var strongConnectionsNumber = 0;
            var contactsNumber = 0;

            agconfig.SetContactsConfig(ref strongConnectionsNumber, ref contactsNumber);

            var contactsLeft = contactsNumber;
            var random = new Random();
            var strongProbabilyFractoin = 0.8;
            var strongConnectionsInterval = strongProbabilyFractoin / strongConnectionsNumber;
            var strongConnectionsIntervalMin = 0.7 * strongConnectionsInterval;
            var strongConnectionsIntervalDiff = strongConnectionsInterval - strongConnectionsIntervalMin;

            var usedIndices = new List<int>();

            double probabilitySum = 0;
            for (int i = 0; i < contactsNumber; i++)
            {
                var currentAgentIndex = random.Next(agents.Count);
                if (usedIndices.Contains(currentAgentIndex))
                {
                    i--;
                    continue;
                }
                usedIndices.Add(currentAgentIndex);
                var currentAgent = agents.ElementAt(currentAgentIndex);
                var probability = 0.0;

                if (i < strongConnectionsNumber)
                {
                    probability = strongConnectionsIntervalMin + random.NextDouble() * strongConnectionsIntervalDiff;
                }
                else
                {
                    if (i == contactsNumber - 1)
                    {
                        probability = 1 - probabilitySum;
                    }

                    else
                    {
                        probability = (1 - probabilitySum) / contactsLeft;
                    }
                }

                if (currentAgent is Talker && i != contactsNumber - 1)
                {
                    var talkerPenalty = probability  *  (random.NextDouble() % 0.0001);
                    probability += talkerPenalty;
                }

                probabilitySum += probability;
                Contacts.Add(currentAgent, probabilitySum); 
                contactsLeft--;
            }
        }

        public override Call InitiateCall(int currentTime)
        {
            var agconfig = new AgentConfigurator("Organizer");
            var callLength = agconfig.GetCallLength();
            var call = base.MakeCall(currentTime, callLength);
            return call;
        }

        public override void UpdateActivityInterval()
        {
            var agconfig = new AgentConfigurator("Talker");
            ActivityInterval = agconfig.SetActivityInterval();
        }
    }

}
