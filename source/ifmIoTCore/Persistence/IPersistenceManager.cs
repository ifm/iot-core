namespace ifmIoTCore.Persistence
{
    public interface IPersistenceManager
    {
        IPersistenceTransaction Begin();
        void Restore();
    }
}