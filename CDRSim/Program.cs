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
                var choice = random.NextDouble();
                if (choice <= 0.5)                      agents.Add(new RegularAgent(i));
                if (choice > 0.5 && choice < 0.85)      agents.Add(new Talker(i));
                if (choice > 0.5 && choice >= 0.85)     agents.Add(new Organizer(i));
            }
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].Initialize(agents);
            }
            var simulationLength = 100000;
        }
    }
}
