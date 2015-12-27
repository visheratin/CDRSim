using MathNet.Numerics.Distributions;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace CDRSim.Simulation
{
    public static class Information
    {
        public static double Importance     { get; set; }
        public static double Relevance      { get; set; }
        public static double Complexity     { get; set; }
        public static double SpreadersPart { get; set; }
        public static SimulationMode Mode   { get; set; }

        static Information()
        {
            var initSection = (NameValueCollection)ConfigurationManager.GetSection("Simulation");
            Mode = (SimulationMode)int.Parse(initSection["SimulationMode"]);

            initSection = (NameValueCollection)ConfigurationManager.GetSection("Information");
            Importance = double.Parse(initSection["Importance"]);
            Relevance = double.Parse(initSection["Relevance"]);
            Complexity = double.Parse(initSection["Complexity"]);
            SpreadersPart = double.Parse(initSection["SpreadersPart"]);

            Console.WriteLine(Importance);
            Console.WriteLine(Relevance);
            Console.WriteLine(Complexity);
            Console.WriteLine(Mode);
        }

        public static double GetRevenance(int time)
        {
            var result = 1 / Math.Exp(time / 100);
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
