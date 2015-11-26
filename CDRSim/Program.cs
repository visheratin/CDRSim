using CDRSim.Entities;
using CDRSim.Entities.Agents;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

namespace CDRSim
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();

            var savePath = @"D:\CallData\";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            var agentsNumber = 1000;//random.Next(5000, 10000);
            var agents = new List<Agent>();
            var initSection = (NameValueCollection)ConfigurationManager.GetSection("Simulation");
            double regularAgentsPart = double.Parse(initSection["RegularAgentsPart"]);
            var talkersPart = regularAgentsPart + double.Parse(initSection["TalkersPart"]);
            var organizersPart = talkersPart + double.Parse(initSection["OrganizersPart"]);

            Console.WriteLine(organizersPart);
            for (int i = 0; i < agentsNumber; i++)
            {
                var choice = random.NextDouble();
                if (choice <= regularAgentsPart)
                    agents.Add(new RegularAgent(i));
                else
                {
                    if (choice < talkersPart)
                        agents.Add(new Talker(i));
                    else
                        agents.Add(new Organizer(i));
                }
            }
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].Initialize(agents);
            }

            var calls = new List<Call>();
            var simulationLength = 86400;
            //visualization stuff
            var callsCounter = 0;

            using (StreamWriter file = new System.IO.StreamWriter(savePath + "edgePerIteration.txt"))
            {
                //for (int j = 0; j < 7; j++)
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

                        if (i % 3600 == 0)
                        {
                            file.WriteLine(callsCounter);
                        // Console.WriteLine();
                        }
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
