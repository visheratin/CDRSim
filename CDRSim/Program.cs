using CDRSim.Entities.Agents;
using CDRSim.Experiments;
using CDRSim.Parameters;
using CDRSim.Simulation;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CDRSim
{
    class Program
    {

        static void Main(string[] args)
        {
            var savePath = @"CallData";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            var random = new Random();
            string name = "RealExperiment";

            for (int percentage = 5; percentage <= 95; percentage+=5)
            {
                ExperimentGlobal.Instance.Init(name);
                var timer = new Stopwatch();
                ExperimentGlobal.Instance.Parameters.Information.Spreaders = 0;
                ExperimentGlobal.Instance.Parameters.Information.SpreadersPart = percentage / 100.0;
                var organizersPart = ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Organizer"];
                var otherAgentsRatio = Math.Round(ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["RegularAgent"] /
                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Talker"], 0);
                var maxAgentChange = Math.Round(otherAgentsRatio / (otherAgentsRatio + 1), 2);
                var minAgentChange = Math.Round(1 / (otherAgentsRatio + 1), 2);
                var difference = 0.0;
                var currentPercentage = percentage / 100.0;
                if(currentPercentage > organizersPart)
                {
                    difference = currentPercentage - organizersPart;
                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Organizer"] = currentPercentage;
                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["RegularAgent"] -= difference * maxAgentChange;
                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Talker"] -= difference * minAgentChange;
                }
                timer.Start();
                var tasks = new Task[1];
                for (int i = 0; i < 1; i++)
                {
                    var index = i;
                    tasks[i] = (new TaskFactory()).StartNew(() =>
                    {
                        var simulation = new CallsNetworkSimulation(ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength,
                        ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber);
                        var saveName = string.Format("{0}_{1}", index, percentage);
                        simulation.Run(saveName);
                    });
                }
                Task.WaitAll(tasks);
                timer.Stop();
                Console.WriteLine(timer.ElapsedMilliseconds/1000);
            }

            


            //Files
            //using (StreamWriter file = new StreamWriter(savePath + "\\edgeList.txt"))
            //{
            //    foreach (var call in simulation.Calls)
            //    {
            //        string type1;
            //        string type2;

            //        if (call.From is RegularAgent)
            //            type1 = "1.";
            //        else if (call.From is Talker)
            //            type1 = "2.";
            //        else
            //            type1 = "3.";

            //        if (call.To is RegularAgent)
            //            type2 = "1.";
            //        else if (call.To is Talker)
            //            type2 = "2.";
            //        else
            //            type2 = "3.";

            //        file.WriteLine(type1 + call.From.Id + " " + type2 + call.To.Id + " " + call.Length + " " + call.Transfer);
            //    }
            //}
        }
    }
}
