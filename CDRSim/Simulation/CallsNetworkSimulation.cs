using CDRSim.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using CDRSim.Entities.Agents;
using System.IO;
using CDRSim.Experiments;
using CDRSim.Helpers;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using CDRSim.Parameters;

namespace CDRSim.Simulation
{
    public class CallsNetworkSimulation
    {
        public Network network;
        private int simulationLength;
        public BlockingCollection<Call> Calls;


        public CallsNetworkSimulation(int simulationLength, int agentsNumber)
        {
            this.simulationLength = simulationLength;
            network = new Network(agentsNumber);
            InitializeAwareAgents();
            Calls = new BlockingCollection<Call>();
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
            var agentsToAware = agents.Take((int)(ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber *
                ExperimentGlobal.Instance.Parameters.Information.SpreadersPart)).ToArray();
            foreach (var agent in agentsToAware)
            {
                agent.Aware = true;
            }
        }

        public void Run(string name, bool inParallel = false)
        {
            Console.WriteLine("Started simulation");
            var fw = new FileWriter(name);
            //fw.WriteContacts(network);
            //var dumpData = new List<int>();
            //for (int i = 0; i < simulationLength; i++)
            //{
            //    foreach (var agent in network.Agents)
            //    {
            //        var call = agent.Check(i);
            //        if (call != null)
            //        {
            //            Calls.Add(call);
            //        }
            //    }
            //    dumpData.Add(network.Agents.Count(a => a.Aware));
            //}
            //using (StreamWriter file = new StreamWriter(name + ".txt"))
            //{
            //    for (int i = 0; i < dumpData.Count; i++)
            //    {
            //        file.WriteLine("{0} {1}", i, dumpData[i]);
            //    }
            //}

            var dumpData = new int[simulationLength];
            //var fw = new FileWriter(name);

            if (!inParallel)
            {
                for (int i = 0; i < simulationLength; i++)
                {
                    //int[] callInfo = new int[4];
                    foreach (var agent in network.Agents)
                    {
                        var call = agent.Check(i);
                        if (call != null)
                        {
                            Calls.Add(call);
                            //if (call.Transfer == 1)
                            //{
                            //    switch (call.From.Type)
                            //    {
                            //        case AgentType.Organizer:
                            //            callInfo[1]++;
                            //            break;
                            //        case AgentType.Regular:
                            //            callInfo[2]++;
                            //            break;
                            //        case AgentType.Talker:
                            //            callInfo[3]++;
                            //            break;
                            //        default:
                            //            break;
                            //    }
                            //}
                        }
                    }
                    //callInfo[0] = network.Agents.Count(a => a.Aware);
                    //if(i > 0)
                    //{
                    //    callInfo[1] += dumpData[i - 1][1];
                    //    callInfo[2] += dumpData[i - 1][2];
                    //    callInfo[3] += dumpData[i - 1][3];
                    //}
                    //dumpData[i] = callInfo;
                    dumpData[i] = network.Agents.Count(a => a.Aware);
                    //fw.CallsCount.Add(Calls.Count);
                }
                fw.WriteDumpData(dumpData);
                GC.Collect();
                //fw.WriteCallsCount();
                //fw.WriteCallsData(Calls);
            }
            else
            {
                var taskAgents = new List<int>[Environment.ProcessorCount];
                for (int j = 0; j < taskAgents.Length; j++)
                {
                    taskAgents[j] = new List<int>();
                }
                var counter = 0;
                while (counter < network.Agents.Count)
                {
                    taskAgents[counter % Environment.ProcessorCount].Add(counter);
                    counter++;
                }
                var tasks = new Task[Environment.ProcessorCount];

                //fw.WriteContacts(network);

                for (int i = 0; i < simulationLength; i++)
                {
                    int[] callInfoTotal = new int[4];
                    for (int j = 0; j < Environment.ProcessorCount; j++)
                    {
                        var agentsList = taskAgents[j];
                        tasks[j] = Task.Factory.StartNew(() =>
                        {
                            foreach (var agent in agentsList)
                            {
                                var call = network.Agents[agent].Check(i);
                                if (call != null)
                                {
                                    Calls.Add(call);
                                }
                            }
                        });
                    }

                    Task.WaitAll(tasks);
                    //callInfoTotal[0] = network.Agents.Count(a => a.Aware);
                    //callInfoTotal[1] = Calls.Count(c => c.Transfer == 1 && c.Start <= i && c.From.Type == AgentType.Regular);
                    //callInfoTotal[2] = Calls.Count(c => c.Transfer == 1 && c.Start <= i && c.From.Type == AgentType.Talker);
                    //callInfoTotal[3] = Calls.Count(c => c.Transfer == 1 && c.Start <= i && c.From.Type == AgentType.Organizer);
                    //dumpData[i] = callInfoTotal;
                    dumpData[i] = network.Agents.Count(a => a.Aware); ;
                    //fw.CallsCount.Add(Calls.Count);
                }
                fw.WriteDumpData(dumpData);
                //fw.WriteCallsCount();
                //fw.WriteCallsData(Calls);
            }

        }
    }
}
