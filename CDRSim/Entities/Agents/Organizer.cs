using System;
using System.Collections.Generic;
using CDRSim.Helpers;
using CDRSim.Parameters;
using System.Collections.Concurrent;

namespace CDRSim.Entities.Agents
{
    public class Organizer : Agent
    {
        public Organizer(int id)
        {
            Id = id;
            Type = AgentType.Organizer;
        }

        public override void Initialize(IEnumerable<Agent> agents)
        {
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            InterestDegree = 0.7 + 0.3 * random.NextDouble();
            base.CreateInitContacts(agents, AgentType.Organizer);
        }

        public override void Create(IEnumerable<Agent> agents, AgentType type, double strongProbabilyFraction, double strongConnectionsIntervalPercent)
        {
            strongProbabilyFraction = 0.65;
            strongConnectionsIntervalPercent = 0.9;
            base.Create(agents, AgentType.Organizer, strongProbabilyFraction, strongConnectionsIntervalPercent);
        }

        public override Call InitiateCall(int currentTime)
        {
            var agconfig = new AgentConfigurator(AgentType.Organizer);
            var callLength = agconfig.GetCallLength();
            var call = base.MakeCall(currentTime, callLength);
            return call;
        }

        public override void UpdateActivityInterval()
        {
            var agconfig = new AgentConfigurator(AgentType.Organizer);
            ActivityInterval = agconfig.SetActivityInterval();
        }
    }


}
