using System;
using System.Collections.Generic;
using System.Linq;
using CDRSim.Helpers;
using CDRSim.Simulation;
using CDRSim.Parameters;

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
            var agconfig = new AgentConfigurator(AgentType.Regular);
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            var activityInterval = agconfig.SetActivityInterval();
            ActivityInterval = random.Next(activityInterval);
            _activateTime = ActivityInterval;

            int strongConnectionsNumber = 0;
            int contactsNumber = 0;

            InterestDegree = random.NextDouble();

            agconfig.SetContactsConfig(ref strongConnectionsNumber, ref contactsNumber);

            var contactsLeft = contactsNumber;
            var strongConnectionsInterval = 0.75 / strongConnectionsNumber;
            var strongConnectionsIntervalMin = 0.8 * strongConnectionsInterval;
            var strongConnectionsIntervalDiff = strongConnectionsInterval - strongConnectionsIntervalMin;

            var total = 1.0;
            var probabilitySum = 0.0;
            var usedAgents = new List<Agent>();
            for (int i = 0; i < contactsNumber; i++)
            {
                Agent currentAgent = null;
                var getContact = random.NextDouble();
                var contactAgents = agents.Where(a => a.Contacts.Keys.Contains(this)).ToList();
                if (getContact > 0.8 && contactAgents.Count > 0)
                {
                    currentAgent = contactAgents[random.Next(0, contactAgents.Count() - 1)];
                }
                else
                {
                    currentAgent = agents.ElementAt(random.Next(agents.Count - 1));
                }
                if (usedAgents.Contains(currentAgent))
                {
                    i--;
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
                    if (i == contactsNumber - 1)
                    {
                        probability = 1 - probabilitySum;
                    }

                    else
                    {
                        probability = (1 - probabilitySum) / contactsLeft;
                    }
                }

                probabilitySum += probability;
                total -= probability;
               
                Contacts.Add(currentAgent, probabilitySum);
                contactsLeft--;

            }
        }

        public override Call InitiateCall(int currentTime)
        {
            var agconfig = new AgentConfigurator(AgentType.Regular);
            var callLength = agconfig.GetCallLength();
            var call = base.MakeCall(currentTime, callLength);
            return call;
        }

        public override void UpdateActivityInterval()
        {
            var agconfig = new AgentConfigurator(AgentType.Regular);
            ActivityInterval = agconfig.SetActivityInterval();
        }
    }
}
