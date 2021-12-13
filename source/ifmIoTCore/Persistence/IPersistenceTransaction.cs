namespace ifmIoTCore.Persistence
{
    using System;
    using System.Collections.Generic;

    public interface IPersistenceTransaction
    {
        event EventHandler Committed;
        event EventHandler Rolledback;

        IEnumerable<ServiceInvocation> ServiceInvocations { get; }
        void Persist(ServiceInvocation serviceInvocation);
        void Unpersist(ServiceInvocation serviceInvocation);
        void Commit();
        void Rollback();
    }
}