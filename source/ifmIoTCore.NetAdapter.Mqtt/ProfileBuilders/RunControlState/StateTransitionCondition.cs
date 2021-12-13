namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders.RunControlState
{
    using System;

    public class StateTransitionCondition
    {
        public Func<bool> Condition { get; }
        public string Description { get; }

        public StateTransitionCondition(Func<bool> condition, string description)
        {
            this.Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}