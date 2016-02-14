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
using System.Linq;

namespace CDRSim
{
    class Program
    {

        static void Main(string[] args)
        {
            var savePath = @"CallData\";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            var random = new Random();

            string name = "RealExperiment";
            var timer = new Stopwatch();
            timer.Start();
            ExperimentGlobal.Instance.Init(name);
                var simulation = new CallsNetworkSimulation(ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength,
                        ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber);
                var saveName = "test";
                simulation.Run(saveName, true);
            timer.Stop();
            Console.WriteLine("Total: {0}", timer.ElapsedMilliseconds);
            Console.WriteLine("Informed: {0}", simulation.network.Agents.Count(a => a.Aware));
            Console.ReadLine();


            //string name = "RealExperiment";
            //for (int spreaders = 0; spreaders <= 2; spreaders++)
            //{
            //    var spreadersFolder = savePath + spreaders.ToString() + "\\";
            //    if (!Directory.Exists(spreadersFolder))
            //    {
            //        Directory.CreateDirectory(spreadersFolder);
            //    }
            //    for (int percentage = 5; percentage <= 95; percentage += 5)
            //    {
            //        var percentageString = String.Format("{0:00}", percentage);
            //        var percentageFolder = spreadersFolder + percentageString + "%\\";
            //        if (!Directory.Exists(percentageFolder))
            //        {
            //            Directory.CreateDirectory(percentageFolder);
            //        }
            //        ExperimentGlobal.Instance.Init(name);
            //        var timer = new Stopwatch();
            //        ExperimentGlobal.Instance.Parameters.Information.Spreaders = spreaders;
            //        ExperimentGlobal.Instance.Parameters.Information.SpreadersPart = percentage / 100.0;
            //        double mainPart = 0.0;
            //        switch (spreaders)
            //        {
            //            case 0:
            //                mainPart = ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Organizer"];
            //                break;
            //            case 1:
            //                mainPart = ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Talker"];
            //                break;
            //            case 2:
            //                mainPart = ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["RegularAgent"];
            //                break;
            //            default:
            //                break;
            //        }
            //        double otherAgentsRatio = 0.0;
            //        switch (spreaders)
            //        {
            //            case 0:
            //                otherAgentsRatio = Math.Round(ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["RegularAgent"] /
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Talker"], 0);
            //                break;
            //            case 1:
            //                otherAgentsRatio = Math.Round(ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["RegularAgent"] /
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Organizer"], 0);
            //                break;
            //            case 2:
            //                otherAgentsRatio = Math.Round(ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Talker"] /
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Organizer"], 0);
            //                break;
            //            default:
            //                break;
            //        }
            //        var maxAgentChange = Math.Round(otherAgentsRatio / (otherAgentsRatio + 1), 2);
            //        var minAgentChange = Math.Round(1 / (otherAgentsRatio + 1), 2);
            //        var difference = 0.0;
            //        var currentPercentage = percentage / 100.0;
            //        if (currentPercentage > mainPart)
            //        {
            //            difference = currentPercentage - mainPart;
            //            switch (spreaders)
            //            {
            //                case 0:
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Organizer"] = currentPercentage;
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["RegularAgent"] -= difference * maxAgentChange;
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Talker"] -= difference * minAgentChange;
            //                    break;
            //                case 1:
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Talker"] = currentPercentage;
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["RegularAgent"] -= difference * maxAgentChange;
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Organizer"] -= difference * minAgentChange;
            //                    break;
            //                case 2:
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["RegularAgent"] = currentPercentage;
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Talker"] -= difference * maxAgentChange;
            //                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Organizer"] -= difference * minAgentChange;
            //                    break;
            //                default:
            //                    break;
            //            }

            //        }
            //        timer.Start();
            //        var experimentsCount = 100;
            //        var tasks = new Task[experimentsCount];
            //        for (int i = 0; i < experimentsCount; i++)
            //        {
            //            var index = i;
            //            tasks[i] = (new TaskFactory()).StartNew(() =>
            //            {
            //                var simulation = new CallsNetworkSimulation(ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength,
            //                ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber);
            //                var saveName = string.Format(percentageFolder + "{0}", index);
            //                simulation.Run(saveName);
            //            });
            //        }
            //        Task.WaitAll(tasks);
            //        timer.Stop();
            //        Console.WriteLine(timer.ElapsedMilliseconds / 1000);
            //    }
            //}
        }
    }
}
