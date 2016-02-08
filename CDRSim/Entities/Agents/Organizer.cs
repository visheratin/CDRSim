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
            Contacts = new Dictionary<Agent, double>();
        }

        public override void Initialize(BlockingCollection<Agent> agents)
        {
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            InterestDegree = 0.7 + 0.3 * random.NextDouble();
            var strongProbabilyFraction = 0.65;
            var strongConnectionsIntervalPercent = 0.9;
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
