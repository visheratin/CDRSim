using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CDRSim.Entities;
using CDRSim.Entities.Agents;

namespace CDRSim.Helpers
{
    class FileWriter
    {
        public string SaveName { get; set; }
        public List<int> CallsCount;

        public FileWriter(string savename)
        {
            SaveName = savename;
            CallsCount = new List<int>();
        }

        public void WriteContacts(Network network)
        {
            using (StreamWriter file = new StreamWriter(SaveName + "contacts" + ".txt"))
            {
                foreach (var agent in network.Agents)
                {
                    if (agent.Aware == true)
                    {
                        file.Write("1." + agent.Id + " ");
                    }
                    else
                    {
                        file.Write("0." + agent.Id + " ");
                    }

                    foreach (var cont in agent.Contacts)
                    {
                        file.Write(cont.Key.Id + " ");
                    }
                    file.Write("\n");
                }

            }

        }

        public void WriteCallsCount()
        {
            using (StreamWriter file = new StreamWriter(SaveName + "edgePerIteration" + ".txt"))
            {
                for (int i = 0; i < CallsCount.Count; i++)
                    file.WriteLine(i + " " + CallsCount[i]);
            }
        }

        public void WriteCallsData(List<Call> Calls)
        {
            using (StreamWriter file = new StreamWriter(SaveName + "edgeList.txt"))
            {
                foreach (var call in Calls)
                {
                    string type1;
                    string type2;

                    if (call.From is RegularAgent)
                        type1 = "1.";
                    else if (call.From is Talker)
                        type1 = "2.";
                    else
                        type1 = "3.";

                    if (call.To is RegularAgent)
                        type2 = "1.";
                    else if (call.To is Talker)
                        type2 = "2.";
                    else
                        type2 = "3.";

                    file.WriteLine(type1 + call.From.Id + " " + type2 + call.To.Id + " " + call.Length + " " + call.Transfer);
                }
            }
        }
    }
}
