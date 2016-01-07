using CDRSim.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRSim.Experiments
{
    public class ExperimentGlobal
    {

        private static ExperimentGlobal instance;
        public ExperimentParameters Parameters { get; private set; }
        public string Name { get; private set; }

        private ExperimentGlobal() { }

        public static ExperimentGlobal Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ExperimentGlobal();
                }
                return instance;
            }
        }

        public void Init(string name)
        {
            Parameters = new ExperimentParameters(name);
            Name = name;
        }

    }
}
