using CDRSim.Experiments;
using MathNet.Numerics.Distributions;
using System;

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
        }

        public static double GetRelevance(int time)
        {
            time -= ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength / 2;
            var result = 1 / (1 + Math.Exp(time / 0.058 * ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength));
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
