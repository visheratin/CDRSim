using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using System.Configuration;
using System.Collections.Specialized;
using CDRSim.Helpers;
using CDRSim.Parameters;

namespace CDRSim.Entities.Agents
{
    public class Organizer : Agent
    {
        public Organizer(int id)
        {
            Id = id;
            Contacts = new Dictionary<Agent, double>();
        }

        public override void Initialize(List<Agent> agents)
        {
            var agconfig = new AgentConfigurator(AgentType.Organizer);
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            var activityInterval = agconfig.SetActivityInterval();
            ActivityInterval = random.Next(activityInterval);
            _activateTime = ActivityInterval;

            InterestDegree = 0.7 + 0.3 * random.NextDouble();

            var strongConnectionsNumber = 0;
            var contactsNumber = 0;

            agconfig.SetContactsConfig(ref strongConnectionsNumber, ref contactsNumber);

            var contactsLeft = contactsNumber;
            var strongProbabilyFraction = 0.65;
            var strongConnectionsInterval = strongProbabilyFraction / strongConnectionsNumber;
            var strongConnectionsIntervalMin = 0.9 * strongConnectionsInterval;
            var strongConnectionsIntervalDiff = strongConnectionsInterval - strongConnectionsIntervalMin;

            var usedAgents = new List<Agent>();

            double probabilitySum = 0;
            
            var contactAgents = agents.Where(a => a.Contacts.Keys.Contains(this) && !this.Contacts.Keys.Contains(a)).ToList();
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
                    currentAgent = agents.ElementAt(random.Next(agents.Count-1));
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

        public override Call InitiateCall(int currentTime)
        {
            var agconfig = new AgentConfigurator(AgentType.Organizer);
            var callLength = agconfig.GetCallLength();
            var call = base.MakeCall(currentTime, callLength);
            return call;
        }

        public override void UpdateActivityInterval()
        {
            var agconfig = new AgentConfigurator(AgentType.Organizer);
            ActivityInterval = agconfig.SetActivityInterval();
        }
    }


}
