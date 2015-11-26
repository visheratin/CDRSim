using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRSim.Helpers
{
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
            var distribution = new Poisson(activityMean);
            var interval = distribution.Sample();
            return (int)interval;
        }

        public int GetCallLength()
        {
            var callLengthMean = int.Parse(section["CallLengthMean"]);
            var callLengthStd = int.Parse(section["CallLengthStd"]);
            var distribution = new Poisson(callLengthMean);
            var length = distribution.Sample();
            return Math.Abs((int)length);
        }

        public void SetContactsConfig(ref int strongConnectionsInterval, ref int contactsNumber)
        {
            var contactsMean = int.Parse(section["ContactsMean"]);
            var contactsStd = int.Parse(section["ContactsStd"]);
            var distribution = new Poisson(contactsMean);
            while (contactsNumber == 0)
            {
                contactsNumber = (int)distribution.Sample();
            }
            var strongConnectionsMean = int.Parse(section["StrongConnectionsMean"]);
            var strongConnectionsStd = int.Parse(section["StrongConnectionsStd"]);
            distribution = new Poisson(strongConnectionsMean);
            strongConnectionsInterval = (int)distribution.Sample();
        }

    }
}
