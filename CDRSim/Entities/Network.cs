using CDRSim.Entities.Agents;
using CDRSim.Experiments;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CDRSim.Entities
{
    public class Network
    {
        public List<Agent> Agents;

        public Network(int agentsNumber = 1000)
        {
            var random = new Random();
            Agents = new List<Agent>();
            var regularAgentsPart = ExperimentGlobal.Instance.Parameters.Simulation
                .AgentTypes.First(a => a.Key == "RegularAgent").Value;
            var talkersPart = regularAgentsPart + ExperimentGlobal.Instance.Parameters.Simulation
                .AgentTypes.First(a => a.Key == "Talker").Value;
            var organizersPart = talkersPart + ExperimentGlobal.Instance.Parameters.Simulation
                .AgentTypes.First(a => a.Key == "Organizer").Value;
            for (int i = 0; i < agentsNumber; i++)
            {
                if (i <= regularAgentsPart*agentsNumber)
                    Agents.Add(new RegularAgent(i));
                else
                {
                    if (i <= talkersPart * agentsNumber)
                        Agents.Add(new Talker(i));
                    else
                    {
                        var agent = new Organizer(i);
                        Agents.Add(agent);
                    }
                }
            }
            foreach (var agent in Agents)
            {
                agent.Initialize(Agents);
            }
            foreach (var agent in Agents)
            {
                agent.Create(Agents, agent.Type, 0, 0);
            }
            //var tasks = new Task[Environment.ProcessorCount];
            //var taskAgents = new List<int>[Environment.ProcessorCount];
            //for (int i = 0; i < taskAgents.Length; i++)
            //{
            //    taskAgents[i] = new List<int>();
            //}
            //var counter = 0;
            //while (counter < Agents.Count)
            //{
            //    taskAgents[counter % Environment.ProcessorCount].Add(counter);
            //    counter++;
            //}
            //var parallelAgents = new BlockingCollection<Agent>();
            //foreach (var agent in Agents)
            //{
            //    parallelAgents.Add(agent);
            //}
            //for (int i = 0; i < Environment.ProcessorCount; i++)
            //{
            //    var agentsList = taskAgents[i];
            //    tasks[i] = Task.Factory.StartNew(() =>
            //    {
            //        foreach (var agent in agentsList)
            //        {
            //            parallelAgents.ElementAt(agent).Initialize(Agents);
            //        }
            //        foreach (var agent in agentsList)
            //        {
            //            parallelAgents.ElementAt(agent).Create(Agents, parallelAgents.ElementAt(agent).Type, 0, 0);
            //        }
            //    });
            //}
            //Task.WaitAll(tasks);
            //Agents = parallelAgents.ToList();
        }
    }
}
