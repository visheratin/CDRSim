using CDRSim.Entities;
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

            var agentsNumber = random.Next(5000, 10000);
            var agents = new List<Agent>();
            for (int i = 0; i < agentsNumber; i++)
            {
                var choice = random.NextDouble();
                if (choice <= 0.5)                      agents.Add(new RegularAgent(i));
                if (choice > 0.5 && choice < 0.85)      agents.Add(new Talker(i));
                if (choice >= 0.85)     agents.Add(new Organizer(i));
            }
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].Initialize(agents);
            }
            var calls = new List<Call>();
            var simulationLength = 1000;
            for (int i = 0; i < simulationLength; i++)
            {
                foreach (var agent in agents)
                {
                    var call = agent.Check(i);
                    if (call != null)
                        calls.Add(call);
                }
            }
        }
    }
}
