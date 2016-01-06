using CDRSim.Entities.Agents;
using CDRSim.Simulation;
using System;
using System.IO;

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

            var agentsNumber = 1000;
            
            // Speed up 10 times for visualization. Also in App.config
            var simulationLength = 8640;
            var simulation = new CallsNetworkSimulation(simulationLength, agentsNumber);
            simulation.Run();
            //Console.ReadLine();
            Console.WriteLine("End");
            using (StreamWriter file = new StreamWriter(savePath + "edgePerIteration.txt"))
            {
                file.WriteLine(simulation.Calls.Count);
            }
            //Files
            using (StreamWriter file = new StreamWriter(savePath + "edgeList.txt"))
            {
                foreach (var call in simulation.Calls)
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
