using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRSim.Parameters
{
    [Serializable]
    public class AgentParameters
    {
        public AgentType Type;
        public double ActivityMean;
        public double ActivityStd;
        public double ContactsMean;
        public double StrongConnectionsMean;
        public double CallLengthMean;
    }
}
