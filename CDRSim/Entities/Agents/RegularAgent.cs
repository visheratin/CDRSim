using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using System.Configuration;
using System.Collections.Specialized;
using CDRSim.Helpers;

namespace CDRSim.Entities.Agents
{
    public class RegularAgent : Agent
    {
        public RegularAgent(int id)
        {
            Id = id;
            Contacts = new Dictionary<Agent, double>();
        }

        public override void Initialize(List<Agent> agents)
        {
            var agconfig = new AgentConfigurator("RegularAgent");
            ActivityInterval = agconfig.SetActivityInterval();
            _activateTime = ActivityInterval;

            int strongConnectionsNumber = 0;
            int contactsNumber = 0;

            agconfig.SetContactsConfig(ref strongConnectionsNumber, ref contactsNumber);

            var strongConnectionsInterval = 0.75 / strongConnectionsNumber;
            var strongConnectionsIntervalMin = 0.8 * strongConnectionsInterval;
            var strongConnectionsIntervalDiff = strongConnectionsInterval - strongConnectionsIntervalMin;

            var random = new Random();
            var total = 1.0;
            var probabilitySum = 0.0;
            var usedIndices = new List<int>();
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
                        probability = total;
                    }
                    else
                    {
                        probability = 0.2 * total;
                    }
                }

                probabilitySum += probability;
                total -= probability;
               
                Contacts.Add(currentAgent, probabilitySum);

            }
        }

        public override Call InitiateCall(int currentTime)
        {
            var agconfig = new AgentConfigurator("RegularAgent");
            var callLength = agconfig.GetCallLength();
            var call = base.MakeCall(currentTime, callLength);
            return call;
        }

        public override void UpdateActivityInterval()
        {
            var agconfig = new AgentConfigurator("RegularAgent");
            ActivityInterval = agconfig.SetActivityInterval();
        }
    }
}
