namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders.RunControlState
{
    using System;

    internal class State
    {
        public Guid Id { get; }
        public object Value { get; }

        public State(Guid id, object value)
        {
            this.Id = id;
            this.Value = value;
        }

        public State(object value) 
            : this(Guid.NewGuid(), value)
        {
        }

        public override string ToString()
        {
            return $" {this.Id}:'{this.Value?.ToString() ?? "null"}'";
        }
    }
}