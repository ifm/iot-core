namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders.RunControlState
{
    using System.Collections.Generic;
    using System.Linq;

    internal class StateTransition
    {
        public State From { get; }
        public State To { get; }
        public IEnumerable<StateTransitionCondition> Conditions { get; }

        public StateTransition(State from, State to) : this(from, to , Enumerable.Empty<StateTransitionCondition>())
        {
        }

        public StateTransition(State from, State to, IEnumerable<StateTransitionCondition> conditions)
        {
            this.From = from;
            this.To = to;
            this.Conditions = conditions;
        }
    }
}