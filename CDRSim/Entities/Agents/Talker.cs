using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using System.Configuration;
using System.Collections.Specialized;

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

            var appSettings = ConfigurationManager.AppSettings;
            ActivityInterval = agconfig.SetActivityInterval();
            

            int strongConnectionsNumber = 0;
            int contactsNumber = 0;

            agconfig.SetContactsConfig(ref strongConnectionsNumber, ref contactsNumber);

            var random = new Random();

            var contactsLeft = contactsNumber;
            var strongProbabilyFractoin = 0.8;
            var strongConnectionsInterval = strongProbabilyFractoin / strongConnectionsNumber;
            var strongConnectionsIntervalMin = 0.7 * strongConnectionsInterval;
            var strongConnectionsIntervalDiff = strongConnectionsInterval - strongConnectionsIntervalMin;
            var averageProbabilityFraction = 1 - strongProbabilyFractoin;

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

                    probability = (averageProbabilityFraction / contactsLeft) * (random.NextDouble() % 0.8);// * random.NextDouble() % 0.0001;

                }

                if (currentAgent is Talker)
                {
                    var talkerPenalty = probability * (random.NextDouble() % 0.2 );
                    probability += talkerPenalty;
                    averageProbabilityFraction -= talkerPenalty;
                    
                }

                Contacts.Add(currentAgent, probability); 
                contactsLeft--;
               

            }
          
        }

        public override Call MakeCall()
        {
            throw new NotImplementedException();
        }
    }

}
