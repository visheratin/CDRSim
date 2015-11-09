using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using System.Configuration;
using System.Collections.Specialized;

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

            int strongConnectionsNumber = 0;
            int contactsNumber = 0;

            agconfig.SetContactsConfig(ref strongConnectionsNumber, ref contactsNumber);

            var strongConnectionsInterval = 0.75 / strongConnectionsNumber;
            var strongConnectionsIntervalMin = 0.8 * strongConnectionsInterval;
            var strongConnectionsIntervalDiff = strongConnectionsInterval - strongConnectionsIntervalMin;

            var random = new Random();
            var total = 1.0;
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

                total -= probability;
               
                Contacts.Add(currentAgent, probability);

            }
        }

        public override Call MakeCall()
        {
            throw new NotImplementedException();
        }
    }

    public class AgentConfigurator
    {
        private string sectionName;
        private NameValueCollection section;

        public AgentConfigurator(string sectionName)
        {
            this.sectionName = sectionName;
            section = (NameValueCollection)ConfigurationManager.GetSection(sectionName);
        }

        public int SetActivityInterval()
        {
            var activityMean = int.Parse(section["ActivityMean"]);
            var activityStd = int.Parse(section["ActivityStd"]);
            var distribution = new Normal(activityMean, activityStd);
            var interval = distribution.Sample();
            return (int)interval;
        }

        public void SetContactsConfig(ref int strongConnectionsInterval,ref int contactsNumber)
        {
            var contactsMean = int.Parse(section["ContactsMean"]);
            var contactsStd = int.Parse(section["ContactsStd"]);
            var distribution = new Normal(contactsMean, contactsStd);
            while (contactsNumber == 0)
            {
                contactsNumber = (int)distribution.Sample();
            }
            var strongConnectionsMean = int.Parse(section["StrongConnectionsMean"]);
            var strongConnectionsStd = int.Parse(section["StrongConnectionsStd"]);
            distribution = new Normal(strongConnectionsMean, strongConnectionsStd);
            strongConnectionsInterval = (int)distribution.Sample();
        }

    }
}
