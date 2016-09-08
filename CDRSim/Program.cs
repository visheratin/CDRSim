using CDRSim.Entities.Agents;
using CDRSim.Experiments;
using CDRSim.Parameters;
using CDRSim.Simulation;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
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

            List<String> experiments = new List<String> { "RealExperiment", "ExtremeExperiment" };
            //List<String> experiments = new List<String> { "ExtremeExperiment" };
            List<int> agentsNumbers = new List<int> { 1000, 1000, 1000, 500, 1000, 5000 };


            List<int> range1 = new List<int> { 0, 10, 20, 30, 40, 50, 60, 70, 80 };
            List<int> range2 = new List<int> { 0, 25, 35, 45, 55, 65, 75 };

            List<int> rangeAmountOfAgentsToDelete = new List<int>();
            List<int> orgscount = new List<int> {0, 0, 0,  79, 159, 799};

            List<String> typeToDelete = new List<String> { "Organizer", "Talker", "RegularAgent", "Organizer", "Organizer", "Organizer" };

            //int experimentCounter = 0;

            ParallelOptions pOptions = new ParallelOptions();
            pOptions.MaxDegreeOfParallelism = Convert.ToInt32(Environment.ProcessorCount * 0.8);

            //int orgscount = 0;

            var savePath = @"CallData\";

            for (int experimentCounter = 0; experimentCounter < 6; experimentCounter++)
            {

                Console.WriteLine("experimentCounter  " + experimentCounter);

                if (experimentCounter < 3) {
                    foreach (var val in range1)
                        rangeAmountOfAgentsToDelete.Add(val);
                }
                    
                else
                {
                    foreach (var val in range2) {
                        double newval = Math.Ceiling(val * orgscount[experimentCounter] / 100d);
                        rangeAmountOfAgentsToDelete.Add(Convert.ToInt32(newval));
                    }
                        
                }

                foreach (var experiment in experiments)
                {

                    foreach (var amountOfAgentsToDelete in rangeAmountOfAgentsToDelete)
                    {
                        //int counter = 0;

                        //for (int i = 0; i < 1; i++)
                        Parallel.For(0,200, pOptions, i =>
                       {

                            Console.WriteLine("i" + i);
                            Console.WriteLine("delete agents   " + amountOfAgentsToDelete);

                           savePath = experimentCounter + @"\" + experiment + @"\" + amountOfAgentsToDelete + @"\" + i + @"CallData\" + "\\";

                           if (!Directory.Exists(savePath))
                               Directory.CreateDirectory(savePath);
                           var random = new Random();

                           //string name = "RealExperiment";
                           ExperimentGlobal.Instance.Init(experiment);
                           var simulation = new CallsNetworkSimulation(ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength,
                                       /*ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber*/ agentsNumbers[experimentCounter], amountOfAgentsToDelete, typeToDelete[experimentCounter]);
                           var saveName = savePath + "test";
                           simulation.Run(saveName, false);

                       });
                    }

                }
                rangeAmountOfAgentsToDelete.Clear();
            }
        }

    }
}


//string name = "ExtremeExperiment";
//ExperimentGlobal.Instance.Init(name);
//var experimentName = "Importance";
//var param = new double[] { 0.2, 0.4, 0.6, 0.8, 1.0 };
////var param = new int[] { 20, 40, 60, 80 };
//var experimentFolder = savePath + experimentName + @"\";
//if (!Directory.Exists(experimentFolder))
//{
//    Directory.CreateDirectory(experimentFolder);
//}
////var param = new int[] { 500, 1000, 5000, 10000 };
//foreach (var value in param)
//{
//    ExperimentGlobal.Instance.Parameters.Information.Importance = value;
//    //ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber = value;
//    var folder = experimentFolder + value.ToString() + @"\";
//    if (!Directory.Exists(folder))
//    {
//        Directory.CreateDirectory(folder);
//    }
//    var experimentsCount = 50;
//    var tasks = new Task[experimentsCount];
//    for (int i = 0; i < experimentsCount; i++)
//    {
//        var index = i;
//        tasks[i] = (new TaskFactory()).StartNew(() =>
//        {
//            var simulation = new CallsNetworkSimulation(ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength,
//            ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber);
//            var saveName = string.Format(folder + "{0}", index);
//            simulation.Run(saveName);
//        });
//    }
//    Task.WaitAll(tasks);
//}

//string name = "ExtremeExperiment";
////var timer = new Stopwatch();
////timer.Start();
//for (int i = 0; i < 50; i++)
//{
//    ExperimentGlobal.Instance.Init(name);
//    ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength = 1800;
//    ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber = 5000;
//    ExperimentGlobal.Instance.Parameters.Information.Complexity = 20;
//    var simulation = new CallsNetworkSimulation(ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength,
//            ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber);
//    simulation.Run(i.ToString(), false);
//}
//timer.Stop();
//Console.WriteLine("Total: {0}", timer.ElapsedMilliseconds);
//Console.ReadLine();


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

//counter++;