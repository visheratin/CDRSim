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
            Contacts = new Dictionary<Agent, double>();
        }

        public override void Initialize(BlockingCollection<Agent> agents)
        {
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            InterestDegree = 0.3 + 0.7 * random.NextDouble();
            var strongProbabilyFraction = 0.8;
            var strongConnectionsIntervalPercent = 0.7;
            base.Create(agents, AgentType.Talker, strongProbabilyFraction, strongConnectionsIntervalPercent);
        }

        public override Call InitiateCall(int currentTime)
        {
            var agconfig = new AgentConfigurator(AgentType.Talker);
            int callLength = agconfig.GetCallLength();
            var call = base.MakeCall(currentTime, callLength);
            return call;
        }

        public override void UpdateActivityInterval()
        {
            var agconfig = new AgentConfigurator(AgentType.Talker);
            ActivityInterval = agconfig.SetActivityInterval();
        }
    }

}
