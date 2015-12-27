﻿using CDRSim.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using CDRSim.Entities.Agents;

namespace CDRSim.Simulation
{
    public class CallsNetworkSimulation
    {
        private Network network;
        private int simulationLength;
        public List<Call> Calls;

        public CallsNetworkSimulation(int simulationLength = 8640, int agentsNumber = 1000)
        {
            this.simulationLength = simulationLength;
            network = new Network(agentsNumber);
            InitializeAwareAgents();
            Calls = new List<Call>();
        }

        private void InitializeAwareAgents()
        {
            var organizers = network.Agents.Where(a => a is Organizer);
            foreach (var agent in organizers)
            {
                agent.Aware = true;
            }
        }

        public void Run()
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
                if (i % 100 == 0)
                {
                    Console.WriteLine("Informed {0} agents of {1}", network.Agents.Count(a => a.Aware), network.Agents.Count);
                }
            }
        }
    }
}
