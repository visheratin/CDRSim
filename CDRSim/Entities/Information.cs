using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRSim.Entities
{
    enum SimulationMode
    {
        CALLGRAPH,
        INFORMATIONTRANSFER
    }

    class Information
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
            Relevance = 0; //double.Parse(initSection["Relevance"]);
            Complexity = double.Parse(initSection["Complexity"]);
            SpreadersPart = double.Parse(initSection["SpreadersPart"]);

            Console.WriteLine(Importance);
            Console.WriteLine(Relevance);
            Console.WriteLine(Complexity);

            Console.WriteLine(Mode);
        }
  
    }
}
