using CDRSim.Entities.Agents;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace CDRSim.Entities
{
    public class Network
    {
        public List<Agent> Agents;

        public Network(int agentsNumber = 1000)
        {
            var random = new Random();
            Agents = new List<Agent>();
            var initSection = (NameValueCollection)ConfigurationManager.GetSection("Simulation");
            double regularAgentsPart = double.Parse(initSection["RegularAgentsPart"]);
            var talkersPart = regularAgentsPart + double.Parse(initSection["TalkersPart"]);
            var organizersPart = talkersPart + double.Parse(initSection["OrganizersPart"]);
            for (int i = 0; i < agentsNumber; i++)
            {
                var choice = random.NextDouble();
                if (choice <= regularAgentsPart)
                    Agents.Add(new RegularAgent(i));
                else
                {
                    if (choice < talkersPart)
                        Agents.Add(new Talker(i));
                    else
                    {
                        var agent = new Organizer(i);
                        Agents.Add(agent);
                    }

                }
            }
            for (int i = 0; i < Agents.Count; i++)
            {
                Agents[i].Initialize(Agents);
            }
        }
    }
}
