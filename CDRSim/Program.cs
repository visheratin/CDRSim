using CDRSim.Entities.Agents;
using System;
using System.Collections.Generic;

namespace CDRSim
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            var agentsNumber = random.Next(500, 1000);
            var agents = new List<Agent>();
            for (int i = 0; i < agentsNumber; i++)
            {
                agents.Add(new RegularAgent(i));
            }
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].Initialize(agents);
            }
            var simulationLength = 100000;
        }
    }
}
