using CDRSim.Experiments;
using MathNet.Numerics.Distributions;
using System;

namespace CDRSim.Simulation
{
    public static class Information
    {

        static Information()
        {
        }

        public static double GetRelevance(int time)
        {
            time -= ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength / 2;
            var result = 1 / (1 + Math.Exp(time / 0.058 * ExperimentGlobal.Instance.Parameters.Simulation.SimulationLength));
            return result;
        }

        public static int GetInfoTransferTime()
        {
            var distribution = new Poisson(ExperimentGlobal.Instance.Parameters.Information.Complexity);
            var length = distribution.Sample();
            return Math.Abs(length);
        }
    }
}
