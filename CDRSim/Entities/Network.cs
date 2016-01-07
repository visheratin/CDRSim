using CDRSim.Entities.Agents;
using CDRSim.Experiments;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace CDRSim.Entities
{
    public class Network
    {
        public List<Agent> Agents;

        public Network(int agentsNumber = 1000)
        {
            var random = new Random();
            Agents = new List<Agent>();
            double regularAgentsPart = ExperimentGlobal.Instance.Parameters.Simulation
                .AgentTypes.First(a => a.Key == "RegularAgent").Value;
            var talkersPart = regularAgentsPart + ExperimentGlobal.Instance.Parameters.Simulation
                .AgentTypes.First(a => a.Key == "Talker").Value;
            var organizersPart = talkersPart + ExperimentGlobal.Instance.Parameters.Simulation
                .AgentTypes.First(a => a.Key == "Organizer").Value;
            for (int i = 0; i < agentsNumber; i++)
            {
                var choice = random.NextDouble();
                if (choice <= regularAgentsPart)
                    Agents.Add(new RegularAgent(i));
                else
                {
                    if (choice < talkersPart)
                        Agents.Add(new Talker(i));
                    else
                    {
                        var agent = new Organizer(i);
                        Agents.Add(agent);
                    }

                }
            }
            for (int i = 0; i < Agents.Count; i++)
            {
                Agents[i].Initialize(Agents);
            }
        }
    }
}
