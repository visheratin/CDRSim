using CDRSim.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using CDRSim.Entities.Agents;
using System.IO;
using CDRSim.Experiments;

namespace CDRSim.Simulation
{
    public class CallsNetworkSimulation
    {
        private Network network;
        private int simulationLength;
        public List<Call> Calls;


        public CallsNetworkSimulation(int simulationLength, int agentsNumber)
        {
            this.simulationLength = simulationLength;
            network = new Network(agentsNumber);
            InitializeAwareAgents();
            Calls = new List<Call>();
        }

        private void InitializeAwareAgents()
        {
            IEnumerable<Agent> agents = null;
            if (ExperimentGlobal.Instance.Parameters.Information.Spreaders == 0)
            {
                agents = network.Agents.Where(a => a is Organizer);
            }
            else if (ExperimentGlobal.Instance.Parameters.Information.Spreaders == 1)
            {
                agents = network.Agents.Where(a => a is Talker);
            }
            else if (ExperimentGlobal.Instance.Parameters.Information.Spreaders == 2)
            {
                agents = network.Agents.Where(a => a is RegularAgent);
            }

            var rand = new Random();
            var agentsToAware = (int)(ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber * 
                ExperimentGlobal.Instance.Parameters.Information.SpreadersPart);
            while (agentsToAware > 0)
            {
                var notAwareAgents = agents.Where(a => !a.Aware);
                if (notAwareAgents.Count() > 0)
                {
                    var index = rand.Next(0, notAwareAgents.Count());
                    notAwareAgents.ElementAt(index).Aware = true;
                    agentsToAware--;
                }
                else
                {
                    break;
                }
            }
        }

        public void Run(string name)
        {
            var dumpData = new Dictionary<int, int>();
            for (int i = 0; i < simulationLength; i++)
            {
                foreach (var agent in network.Agents)
                {
                    var call = agent.Check(i);
                    if (call != null)
                    {
                        Calls.Add(call);
                    }
                }
                dumpData.Add(i, network.Agents.Count(a => a.Aware));
            }
            using (StreamWriter file = new StreamWriter(name + ".txt"))
            {
                foreach (var item in dumpData)
                {
                    file.WriteLine("{0} {1}", item.Key, item.Value);
                }
            }
        }
    }
}
