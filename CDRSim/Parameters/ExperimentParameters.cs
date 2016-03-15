using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CDRSim.Parameters
{
    [Serializable]
    public class ExperimentParameters
    {
        public SimulationParameters Simulation;
        public InformationParameters Information;
        public List<AgentParameters> Agents;

        public ExperimentParameters() { }

        public ExperimentParameters(string name)
        {
            var doc = XDocument.Load("config.xml");
            var experimentElement = doc.Root.Elements().FirstOrDefault(e => e.Attribute(XName.Get("name")).Value == name);
            if (experimentElement == null)
                return;
            XmlSerializer serializer = new XmlSerializer(typeof(ExperimentParameters));

            using (var reader = experimentElement.CreateReader())
            {
                var param = (ExperimentParameters)serializer.Deserialize(reader);
                this.Simulation = param.Simulation;
                this.Information = param.Information;
                this.Agents = param.Agents;
            }
        }
    }
}
