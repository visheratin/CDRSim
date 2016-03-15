using System;
using System.Collections.Generic;
using CDRSim.Helpers;
using CDRSim.Parameters;
using System.Collections.Concurrent;

namespace CDRSim.Entities.Agents
{
    public class Talker : Agent
    {
        public Talker(int id)
        {
            Id = id;
            Type = AgentType.Talker;
        }

        public override void Initialize(IEnumerable<Agent> agents)
        {
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            InterestDegree = 0.3 + 0.7 * random.NextDouble();
            base.CreateInitContacts(agents, AgentType.Talker);
        }

        public override void Create(IEnumerable<Agent> agents, AgentType type, double strongProbabilyFraction, double strongConnectionsIntervalPercent)
        {
            strongProbabilyFraction = 0.4;
            strongConnectionsIntervalPercent = 0.7;
            base.Create(agents, AgentType.Talker, strongProbabilyFraction, strongConnectionsIntervalPercent);
        }

        public override Call InitiateCall(int currentTime)
        {
            int callLength = Config.GetCallLength();
            var call = base.MakeCall(currentTime, callLength);
            return call;
        }

        public override void UpdateActivityInterval()
        {
            ActivityInterval = Config.SetActivityInterval();
        }
    }

}
