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
            Contacts = new Dictionary<Agent, double>();
        }

        public override void Initialize(BlockingCollection<Agent> agents)
        {
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            InterestDegree = random.NextDouble();
            var strongProbabilyFraction = 0.75;
            var strongConnectionsIntervalPercent = 0.8;
            base.Create(agents, AgentType.Regular, strongProbabilyFraction, strongConnectionsIntervalPercent);
        }

        public override Call InitiateCall(int currentTime)
        {
            var agconfig = new AgentConfigurator(AgentType.Regular);
            var callLength = agconfig.GetCallLength();
            var call = base.MakeCall(currentTime, callLength);
            return call;
        }

        public override void UpdateActivityInterval()
        {
            var agconfig = new AgentConfigurator(AgentType.Regular);
            ActivityInterval = agconfig.SetActivityInterval();
        }
    }
}
