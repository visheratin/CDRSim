using CDRSim.Helpers;
using CDRSim.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRSim.Parameters
{
    [Serializable]
    public class SimulationParameters
    {
        public XmlSerializableDictionary<string, double> AgentTypes;
        public SimulationMode SimulationMode;
        public int SimulationLength;
        public int AgentsNumber;
        public bool IsCritical;
    }
}
