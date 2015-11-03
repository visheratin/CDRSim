using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using System.Configuration;

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
            var appSettings = ConfigurationManager.AppSettings;
            var activityMean = int.Parse(appSettings["RegularAgentActivityMean"]);
            var activityStd = int.Parse(appSettings["RegularAgentActivityStd"]);
            var distribution = new Normal(activityMean, activityStd);
            var interval = distribution.Sample();
            ActivityInterval = (int)interval;

            var contactsMean = int.Parse(appSettings["RegularAgentContactsMean"]);
            var contactsStd = int.Parse(appSettings["RegularAgentContactsStd"]);
            distribution = new Normal(contactsMean, contactsStd);
            var contactsNumber = 0;
            while (contactsNumber == 0)
            {
                contactsNumber = (int)distribution.Sample();
            }
            var strongConnectionsMean = int.Parse(appSettings["RegularAgentStrongConnectionsMean"]);
            var strongConnectionsStd = int.Parse(appSettings["RegularAgentStrongConnectionsStd"]);
            distribution = new Normal(strongConnectionsMean, strongConnectionsStd);
            var strongConnectionsNumber = (int)distribution.Sample();
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
                    if(i == contactsNumber - 1)
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
}
