using CDRSim.Entities.Agents;
using CDRSim.Experiments;
using CDRSim.Parameters;
using CDRSim.Simulation;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CDRSim
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            ExperimentGlobal.Instance.Init("RealTimeExperiment");

            var savePath = @"CallData\";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            var simulation = new CallsNetworkSimulation(ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength, 
                ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber);
            simulation.Run();
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
