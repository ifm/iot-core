namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders.RunControlState
{
    using System.Collections.Generic;

    internal class EqualityComparerState : IEqualityComparer<State> {
        public bool Equals(State x, State y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(State obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}