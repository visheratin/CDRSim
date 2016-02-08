using CDRSim.Entities;
using CDRSim.Experiments;
using CDRSim.Parameters;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace CDRSim.Helpers
{
    public class AgentConfigurator
    {
        AgentParameters parameters;

        public AgentConfigurator(AgentType type)
        {
            parameters = ExperimentGlobal.Instance.Parameters.Agents.First(a => a.Type == type);
        }

        public int SetActivityInterval()
        {
            var activityMean = parameters.ActivityMean;
            var activityStd = parameters.ActivityStd;
            var distribution = new Normal(activityMean, activityStd);
            var interval = -1.0;
            while(interval < 0)
                interval = distribution.Sample();
            return (int)interval;
        }

        public int GetCallLength()
        {
            Poisson distribution;
            var callLengthMean = parameters.CallLengthMean;
            distribution = new Poisson(callLengthMean);
            var length = distribution.Sample();
            return Math.Abs(length);
        }

        public void SetContactsConfig(ref int strongConnectionsInterval, ref int contactsNumber)
        {
            var contactsMean = parameters.ContactsMean;
            var distribution = new Poisson(contactsMean);
            while (contactsNumber == 0)
            {
                contactsNumber = (int)distribution.Sample();
            }
            var strongConnectionsMean = parameters.StrongConnectionsMean;
            distribution = new Poisson(strongConnectionsMean);
            strongConnectionsInterval = (int)distribution.Sample();
        }

    }
}
