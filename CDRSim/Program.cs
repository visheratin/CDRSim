using CDRSim.Entities;
using CDRSim.Entities.Agents;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

namespace CDRSim
{


    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();

            var savePath = @"CallData\";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            var agentsNumber = 1000;//random.Next(5000, 10000);
            var agents = new List<Agent>();
            var informedAgents = new List<Agent>();
            var initSection = (NameValueCollection)ConfigurationManager.GetSection("Simulation");
            double regularAgentsPart = double.Parse(initSection["RegularAgentsPart"]);
            var talkersPart = regularAgentsPart + double.Parse(initSection["TalkersPart"]);
            var organizersPart = talkersPart + double.Parse(initSection["OrganizersPart"]);
            var spreadersAmount = Math.Round(agentsNumber * Information.SpreadersPart);

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
                    {
                        var agent = new Organizer(i);

                        if (informedAgents.Count < spreadersAmount)
                        {
                            agent.Aware = true;
                            informedAgents.Add(agent);
                        }
                            
                        agents.Add(agent);
                    }

                }
            }
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].Initialize(agents);
            }

            Console.WriteLine("Simulation starts with {0} aware people", informedAgents.Count);


            var calls = new List<Call>();
            // Speed up 10 times for visualization. Also in App.config
            var simulationLength = 8640;
            //var simulationLength = 86400;
            //visualization stuff
            var callsCounter = 0;
            var agentsToCall = new List<Agent>();

            // Relevance decrement stuff
            //The attenuation coefficient
            double betta = 0.05;
            double amplitude = Information.Importance;
            //double currentRelevance = 0;
            double decrement = 0.0;

            using (StreamWriter file = new System.IO.StreamWriter(savePath + "edgePerIteration.txt"))
            {
                //for (int j = 0; j < 7; j++)
                {
                    if (Information.Mode == SimulationMode.CALLGRAPH)
                    {
                        for (int i = 0; i < simulationLength; i++)
                        {

                            foreach (var agent in agents)
                            {
                                var call = agent.Check(i);
                                if (call != null)
                                {
                                    calls.Add(call);
                                    agentsToCall.Add(call.From);
                                    callsCounter++;
                                }
                            }
                        }
                    }

                    else if (Information.Mode == SimulationMode.INFORMATIONTRANSFER)
                    {
                        for (int i = 0; i < simulationLength; i++)
                        {
                            var tempInformedAgents = new List<Agent>();

                            //Relevance decrement
                            if (i < 1000)
                            {
                                decrement = amplitude * Math.Exp(-betta * i / 10) * Math.Cos(i / 10);
                                Information.Relevance = Math.Abs(decrement);
                                //Console.WriteLine(Math.Round(Information.Relevance, 8));
                            }

                            foreach (var agent in informedAgents)
                            {
                                var call = agent.Check(i);
                                if (call != null)
                                {
                                    calls.Add(call);
                                    agentsToCall.Add(call.From);

                                    if (!informedAgents.Contains(call.To) && call.To.Aware == true)
                                        tempInformedAgents.Add(call.To);

                                    callsCounter++;
                                }

                            }

                            foreach (var agent in tempInformedAgents)
                            {
                                informedAgents.Add(agent);
                            }

                        }
                    }

                        //if (i % 3600 == 0)
                        {
                            file.WriteLine(callsCounter);
                        // Console.WriteLine();
                        }
                    }

                
            }
            var groupedAgents = agentsToCall.GroupBy(a => a.Id);
            foreach (var agent in groupedAgents)
            {
                var type = agent.First().GetType();
                //Console.WriteLine("{0} - {1} calls", type.Name, agent.Count());
            }
            //Console.ReadLine();
            //Console.WriteLine(agents.Count);
            //Console.WriteLine(calls.Count);

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

            var counter = 0;
            foreach (var agent in agents) {
                if (agent.Aware == true)
                    counter++;
            }
            Console.WriteLine("{0} people of {1} are aware", counter, agents.Count);
            Console.WriteLine(informedAgents.Count);
        }
    }
}
