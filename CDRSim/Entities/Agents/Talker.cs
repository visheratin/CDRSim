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
    public class Talker : Agent
    {
        public Talker(int id)
        {
            Id = id;
            Contacts = new Dictionary<Agent, double>();
        }

        public override void Initialize(List<Agent> agents)
        {
            var agconfig = new AgentConfigurator(AgentType.Talker);
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            var activityInterval = agconfig.SetActivityInterval();
            ActivityInterval = random.Next(activityInterval);
            _activateTime = ActivityInterval;

            var strongConnectionsNumber = 0;
            var contactsNumber = 0;

            InterestDegree = 0.3 + 0.7 * random.NextDouble();

            agconfig.SetContactsConfig(ref strongConnectionsNumber, ref contactsNumber);

            var contactsLeft = contactsNumber;
            var strongProbabilyFractoin = 0.8;
            var strongConnectionsInterval = strongProbabilyFractoin / strongConnectionsNumber;
            var strongConnectionsIntervalMin = 0.7 * strongConnectionsInterval;
            var strongConnectionsIntervalDiff = strongConnectionsInterval - strongConnectionsIntervalMin;

            var usedAgents = new List<Agent>();

            double probabilitySum = 0;
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
            var agconfig = new AgentConfigurator(AgentType.Talker);
            int callLength = agconfig.GetCallLength();
            var call = base.MakeCall(currentTime, callLength);
            return call;
        }

        public override void UpdateActivityInterval()
        {
            var agconfig = new AgentConfigurator(AgentType.Talker);
            ActivityInterval = agconfig.SetActivityInterval();
        }
    }

}
