namespace ifmIoTCore.Common
{
    using System.Threading;

    public class SingleShotGuard
    {
        private const int Unlocked = 0;
        private const int Locked = 1;

        private int _state;

        public bool CheckAndLock()
        {
            return Interlocked.CompareExchange(ref _state, Locked, Unlocked) == Unlocked;
        }

        public void Unlock()
        {
            Interlocked.CompareExchange(ref _state, Unlocked, Locked);
        }
    }
}
