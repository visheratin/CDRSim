using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using System.Configuration;
using System.Collections.Specialized;

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
            var agconfig = new AgentConfigurator("Organizer");


            var appSettings = ConfigurationManager.AppSettings;
            ActivityInterval = agconfig.SetActivityInterval();


            int strongConnectionsNumber = 0;
            int contactsNumber = 0;
            
            agconfig.SetContactsConfig(ref strongConnectionsNumber, ref contactsNumber);

            var strongProbabilyFractoin = 0.65;
            var strongConnectionsInterval = strongProbabilyFractoin / strongConnectionsNumber;
            var strongConnectionsIntervalMin = 0.9 * strongConnectionsInterval;
            var strongConnectionsIntervalDiff = strongConnectionsInterval - strongConnectionsIntervalMin;
            var averageProbabilityFraction = 1 - strongProbabilyFractoin;

            var random = new Random();
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
                    probability = averageProbabilityFraction / contactsNumber + random.NextDouble() % 0.0001;
                }
                
                Contacts.Add(currentAgent, probability);
                
            }

        }


        public override Call MakeCall()
        {
            throw new NotImplementedException();
        }
    }


}
