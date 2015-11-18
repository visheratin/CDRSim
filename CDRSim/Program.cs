using CDRSim.Entities;
using CDRSim.Entities.Agents;
using System;
using System.Collections.Generic;
using System.IO;

namespace CDRSim
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();

            var savePath = @"D:\CallData\";

            var agentsNumber = 1000;//random.Next(5000, 10000);
            var agents = new List<Agent>();
            for (int i = 0; i < agentsNumber; i++)
            {
                var choice = random.NextDouble();
                if (choice <= 0.5) agents.Add(new RegularAgent(i));
                if (choice > 0.5 && choice < 0.85) agents.Add(new Talker(i));
                if (choice >= 0.85) agents.Add(new Organizer(i));
            }
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].Initialize(agents);
            }

            var calls = new List<Call>();
            var simulationLength = 10000;
            //visualization stuff
            var callsCounter = 0;

            using (StreamWriter file = new System.IO.StreamWriter(savePath + "edgePerIteration.txt"))
            {

                for (int i = 0; i < simulationLength; i++)
                {
                    foreach (var agent in agents)
                    {
                        var call = agent.Check(i);
                        if (call != null)
                        {
                            calls.Add(call);
                            callsCounter++;
                        }
                    }

                    if (callsCounter > 0)
                    {
                        file.WriteLine(callsCounter);
                    }

                }
            }
            Console.WriteLine(agents.Count);
            Console.WriteLine(calls.Count);

            //Files
            using (StreamWriter file = new System.IO.StreamWriter(savePath + "edgeList.txt"))
            {
                foreach (var call in calls)
                {
                    string type1;
                    string type2;

                    if (call.From is RegularAgent)
                        type1 = "1.";
                    else if(call.From is Talker)
                        type1 = "2.";
                    else
                        type1 = "3.";

                    if (call.To is RegularAgent)
                        type2 = "1.";
                    else if (call.To is Talker)
                        type2 = "2.";
                    else 
                        type2 = "3.";

                    file.WriteLine(type1 + call.From.Id + " " + type2 + call.To.Id + " " + call.Length);
                }
            }
        }
    }
}
