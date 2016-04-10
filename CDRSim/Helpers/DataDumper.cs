using CDRSim.Entities;
using CDRSim.Entities.Agents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRSim.Helpers
{
    public class DataDumper
    {
        private string _prefix;

        public DataDumper(string savePrefix)
        {
            _prefix = savePrefix;
        }

        public void DumpAgents(List<Agent> agents)
        {
            var filename = _prefix + "-agents.txt";
            using (var stream = new FileStream(filename, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            {
                foreach (var agent in agents)
                {
                    var type = "";
                    switch (agent.Type)
                    {
                        case Parameters.AgentType.All:
                            type = "All";
                            break;
                        case Parameters.AgentType.Regular:
                            type = "Regular";
                            break;
                        case Parameters.AgentType.Talker:
                            type = "Busy";
                            break;
                        case Parameters.AgentType.Organizer:
                            type = "Organizer";
                            break;
                        default:
                            break;
                    }
                    writer.WriteLine(agent.Id + " " + type + " " + agent.Contacts.Length);
                }
            }
        }

        public void DumpContacts(List<Agent> agents)
        {
            var filename = _prefix + "-contacts.txt";
            using (var stream = new FileStream(filename, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            {
                foreach (var agent in agents)
                {
                    for (int i = 0; i < agent.Contacts.Length; i++)
                    {
                        writer.WriteLine(agent.Id + " " + agent.Contacts[i].Id + " " + agent.RealContactProbabilities[i]);
                    }
                }
            }
        }

        public void DumpCalls(List<Call> calls)
        {
            var filename = _prefix + "-calls.txt";
            using (var stream = new FileStream(filename, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            {
                foreach (var call in calls)
                {
                    writer.WriteLine(call.Id + " " + call.From.Id + " " + call.To.Id + " " + call.Length + " " + call.Transfer);
                }
            }
        }
    }
}
