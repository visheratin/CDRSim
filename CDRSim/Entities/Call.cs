using CDRSim.Entities.Agents;

namespace CDRSim.Entities
{
    public class Call
    {
        public int Id { get; private set; }
        public Agent From { get; private set; }
        public Agent To { get; private set; }
        public int Start { get; private set; }
        public int Finish { get { return Start + Length; } }
        public int Length { get; private set; }

        public Call(Agent from, Agent to, int start, int length)
        {
            From = from;
            To = to;
            Start = start;
            Length = length;
        }
    }
}
