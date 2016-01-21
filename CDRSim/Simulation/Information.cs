using CDRSim.Experiments;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

namespace CDRSim.Simulation
{
    public static class Information
    {
        public static double Importance     { get; set; }
        public static double Relevance      { get; set; }
        public static double Complexity     { get; set; }
        public static double SpreadersPart { get; set; }
        public static SimulationMode Mode   { get; set; }
        public static double Spreaders         { get; set; }

        static Information()
        {
            Mode = ExperimentGlobal.Instance.Parameters.Simulation.SimulationMode;
            
            Importance = ExperimentGlobal.Instance.Parameters.Information.Importance;
            Relevance = ExperimentGlobal.Instance.Parameters.Information.Relevance;
            Complexity = ExperimentGlobal.Instance.Parameters.Information.Complexity;
            SpreadersPart = ExperimentGlobal.Instance.Parameters.Information.SpreadersPart;
            Spreaders = ExperimentGlobal.Instance.Parameters.Information.Spreaders;

            Console.WriteLine(Importance);
            Console.WriteLine(Relevance);
            Console.WriteLine(Complexity);
            Console.WriteLine(Mode);
        }

        public static double GetRevenance(int time)
        {
            time -= 42000;
            var result = 1 / (1 + Math.Exp(time/5000));

            //using (StreamWriter file = new StreamWriter("time.txt", true))
            //{
            //    file.WriteLine(time + " " + result);
            //}

            return result;
        }

        public static int GetInfoTransferTime()
        {
            var distribution = new Poisson(Complexity);
            var length = distribution.Sample();
            return Math.Abs(length);
        }
    }
}
