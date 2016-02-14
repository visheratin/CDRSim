using System;
using System.Collections.Generic;
using CDRSim.Helpers;
using CDRSim.Parameters;
using System.Collections.Concurrent;

namespace CDRSim.Entities.Agents
{
    public class RegularAgent : Agent
    {
        public RegularAgent(int id)
        {
            Id = id;
            Type = AgentType.Regular;
        }

        public override void Initialize(IEnumerable<Agent> agents)
        {
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            InterestDegree = random.NextDouble();
            base.CreateInitContacts(agents, AgentType.Regular);
        }

        public override void Create(IEnumerable<Agent> agents, AgentType type, double strongProbabilyFraction, double strongConnectionsIntervalPercent)
        {
            strongProbabilyFraction = 0.85;
            strongConnectionsIntervalPercent = 0.8;
            base.Create(agents, AgentType.Regular, strongProbabilyFraction, strongConnectionsIntervalPercent);
        }

        public override Call InitiateCall(int currentTime)
        {
            var callLength = Config.GetCallLength();
            var call = base.MakeCall(currentTime, callLength);
            return call;
        }

        public override void UpdateActivityInterval()
        {
            ActivityInterval = Config.SetActivityInterval();
        }
    }
}
