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
            if (Information.Spreaders == 0)
            {
                var organizers = network.Agents.Where(a => a is Organizer);
                foreach (var agent in organizers)
                {
                    agent.Aware = true;
                }
            }

            else if (Information.Spreaders == 1)
            {

                var rand = new Random();
                var regulars = network.Agents.Where(a => a is RegularAgent);
                var c = 0;
                var percentOfSpreaders = Information.SpreadersPart / 100;
                double fate = (100 / (78 / percentOfSpreaders));
                fate /= 100;
                Console.WriteLine(fate);

                foreach (var agent in regulars)
                {
                    var choice = rand.NextDouble();
                    if (choice < fate)
                    {
                        agent.Aware = true;
                        c++;
                    }
                }
                Console.WriteLine(c);

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

        public void Run(int c, string name)
        {
            var savePath = @"CallData\";
            using (StreamWriter file = new StreamWriter(savePath + Information.Importance + name + c + ".txt"))
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
