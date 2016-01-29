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

            //ExperimentGlobal.Instance.Init(name);
            //var simulation = new CallsNetworkSimulation(ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength,
            //            ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber);
            //var saveName = "test";
            //simulation.Run(saveName);

            for (int percentage = 5; percentage <= 95; percentage += 5)
            {
                ExperimentGlobal.Instance.Init(name);
                var timer = new Stopwatch();
                ExperimentGlobal.Instance.Parameters.Information.Spreaders = 2;
                ExperimentGlobal.Instance.Parameters.Information.SpreadersPart = percentage / 100.0;
                var organizersPart = ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Talker"];
                var otherAgentsRatio = Math.Round(ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["RegularAgent"] /
                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Organizer"], 0);
                var maxAgentChange = Math.Round(otherAgentsRatio / (otherAgentsRatio + 1), 2);
                var minAgentChange = Math.Round(1 / (otherAgentsRatio + 1), 2);
                var difference = 0.0;
                var currentPercentage = percentage / 100.0;
                if (currentPercentage > organizersPart)
                {
                    difference = currentPercentage - organizersPart;
                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Talker"] = currentPercentage;
                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["RegularAgent"] -= difference * maxAgentChange;
                    ExperimentGlobal.Instance.Parameters.Simulation.AgentTypes["Organizer"] -= difference * minAgentChange;
                }
                timer.Start();
                var tasks = new Task[100];
                for (int i = 0; i < 100; i++)
                {
                    var index = i;
                    tasks[i] = (new TaskFactory()).StartNew(() =>
                    {
                        var simulation = new CallsNetworkSimulation(ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength,
                        ExperimentGlobal.Instance.Parameters.Simulation.AgentsNumber);
                        var saveName = string.Format("{0}_{1}", percentage, index);
                        simulation.Run(saveName);
                    });
                }
                Task.WaitAll(tasks);
                timer.Stop();
                Console.WriteLine(timer.ElapsedMilliseconds / 1000);
            }
        }
    }
}
