using System.Collections.Generic;

namespace CDRSim.Entities.Agents
{
    public abstract class Agent
    {
        public int Id { get; set; }
        public Dictionary<Agent, double> Contacts { get; set; }
        public int ActivityInterval { get; set; }
        public bool Busy { get; set; }

        public abstract void Initialize(List<Agent> agents);
        public abstract Call MakeCall();
 

    }
}
