using System;
using System.Collections.Generic;

namespace CDRSim.Entities.Agents
{
    public abstract class Agent
    {
        public int Id { get; set; }
        public Dictionary<Agent, double> Contacts { get; set; }
        public int ActivityInterval { get; set; }
        public int _activateTime { get; set; }
        public int _callEndTime { get; set; }
        public double InterestDegree { get; set; }
        public bool Busy { get; set; }
        public bool Aware { get; set; }


        Random random = new Random();

        public abstract void Initialize(List<Agent> agents);
        public abstract void UpdateActivityInterval();
        public abstract Call InitiateCall(int currentTime);
        public virtual Call MakeCall(int currentTime, int length)
        {
            var random = new Random((int)DateTime.Now.ToBinary() + Id);
            Agent agentToCall = null;
            double agentToCallTie = 0;
            var randomValue = random.NextDouble();
            foreach (var contact in Contacts)
            {
                if (randomValue < contact.Value)
                {
                    if (!contact.Key.Busy)
                    {
                        agentToCall = contact.Key;
                        agentToCallTie = contact.Value;
                    }
                        
                    break;
                }
            }
            if (agentToCall != null)
            {
                if (this is Talker && agentToCall is Talker)
                    length *= 3;
                var call = new Call(this, agentToCall, currentTime, length);
                _callEndTime = currentTime + length;
                Busy = true;
                agentToCall.Busy = true;
                UpdateActivityInterval();
                _activateTime = _callEndTime + ActivityInterval;
                if (Information.Mode == SimulationMode.INFORMATIONTRANSFER)
                    agentToCall.ReceiveInformation(Information.Importance, Information.Relevance, Information.Complexity, agentToCallTie);
                return call;
            }
            return null;
        }
 
        public virtual Call Check(int currentTime)
        {
            if (currentTime >= _callEndTime)
                Busy = false;
            if (currentTime >= _activateTime)
                return InitiateCall(currentTime);
            else
                return null;
        }

        public virtual void ReceiveInformation(double trandferProbability, double relevance, double complexity, double contactTie)
        {

            var choice = random.NextDouble();
            if (choice <= trandferProbability)
            {
                if (trandferProbability >= 0.85 && relevance >= InterestDegree)
                {
                    Aware = true;
                }

                else
                {
                    if (contactTie > 0.5)
                    {
                        Aware = true;
                    }
                }

            }
                

        }
    }
}
