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
            var p = ExperimentGlobal.Instance.Parameters;
            this.simulationLength = simulationLength;
            network = new Network(agentsNumber);
            InitializeAwareAgents();
            Calls = new List<Call>();
        }

        private void InitializeAwareAgents()
        {
            IEnumerable<Agent> agents = null;
            if (Information.Spreaders == 0)
            {
                agents = network.Agents.Where(a => a is Organizer);
            }
            else if (Information.Spreaders == 1)
            {
                agents = network.Agents.Where(a => a is RegularAgent);
            }
            else if (Information.Spreaders == 2)
            {
                agents = network.Agents.Where(a => a is Talker);
            }

            var rand = new Random();
            var agentsToAware = (int)(ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber * Information.SpreadersPart);
            while (agentsToAware > 0)
            {
                var notAwareAgents = agents.Where(a => !a.Aware);
                var index = rand.Next(0, notAwareAgents.Count());
                notAwareAgents.ElementAt(index).Aware = true;
                agentsToAware--;
            }


            //one agent
            //var organizers = network.Agents.Where(a => a is Organizer);
            //var counter = 0;

            //foreach (var agent in organizers)
            //{
            //    if (counter == 1)
            //    {
            //        agent.Aware = true;

            //    }
            //    counter++;

            //}



        }

        public void Run(string name)
        {
            var savePath = @"CallData\";
            using (StreamWriter file = new StreamWriter(savePath + name + ".txt"))
            {
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

                    //using (StreamWriter file1 = new StreamWriter(savePath + "\\edgePerIteration.txt", true))
                    //{
                    //    file1.WriteLine(Calls.Count);
                    //}
                    ////using (StreamWriter file1 = new StreamWriter(savePath + "\\organizers.txt", true))
                    ////{
                    ////    file1.WriteLine("{0} {1}", i, network.Agents.Count(a => a.Aware && a is Organizer));
                    ////}
                    ////using (StreamWriter file1 = new StreamWriter(savePath + "\\regulars.txt", true))
                    ////{
                    ////    file1.WriteLine("{0} {1}", i, network.Agents.Count(a => a.Aware && a is RegularAgent));
                    ////}
                    ////using (StreamWriter file1 = new StreamWriter(savePath + "\\talkers.txt", true))
                    ////{
                    ////    file1.WriteLine("{0} {1}", i, network.Agents.Count(a => a.Aware && a is Talker));
                    ////}

                    file.WriteLine("{0} {1}", i, network.Agents.Count(a => a.Aware));
                }
            }
        }
    }
}
