using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CDRSim.Entities;
using CDRSim.Entities.Agents;
using System.Collections.Concurrent;
using CDRSim.Parameters;

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

        public void WriteDumpData(int[] data)
        {
            using (StreamWriter file = new StreamWriter(SaveName + ".txt"))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    var item = data[i];
                    file.WriteLine("{0}", item);
                }
            }
        }

        public void WriteDumpDataExt(int[][] data)
        {
            using (StreamWriter file = new StreamWriter(SaveName + "-ext.txt"))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    var item = data[i];
                    file.WriteLine("{0} {1} {2} {3} {4}", item[0], item[1], item[2], item[3], item[4]);
                }
            }
        }

        public void WriteContacts(Network network)
        {
            using (StreamWriter file = new StreamWriter(SaveName + "contacts" + ".txt"))
            {
                foreach (var agent in network.Agents)
                {
                    if (agent.Aware)
                    {
                        file.Write("1." + agent.Id + " ");
                    }
                    else
                    {
                        file.Write("0." + agent.Id + " ");
                    }

                    for (int i = 0; i < agent.Contacts.Length; i++)
                    {
                        file.Write(agent.Contacts[i].Id + "-" + agent.RealContactProbabilities[i] + " ");
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

        public void WriteCallsData(IEnumerable<Call> Calls)
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
