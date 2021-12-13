namespace ifmIoTCore.Persistence
{
    using System;
    using System.Collections.Generic;
    using Utilities;

    public class PersistenceTransaction: IPersistenceTransaction
    {
        private List<ServiceInvocation> _serviceInvocations = new List<ServiceInvocation>();

        public event EventHandler Committed;
        public event EventHandler Rolledback;

        public IEnumerable<ServiceInvocation> ServiceInvocations => this._serviceInvocations;

        public void Persist(ServiceInvocation serviceInvocation)
        {
            this._serviceInvocations.Add(serviceInvocation);
        }

        public void Unpersist(ServiceInvocation serviceInvocation)
        {
            this._serviceInvocations.Remove(serviceInvocation);
        }

        public void Commit()
        {
            this.Committed.Raise(this);
        }

        public void Rollback()
        {
            this._serviceInvocations.Clear();
            this.Rolledback.Raise(this);
        }
    }
}