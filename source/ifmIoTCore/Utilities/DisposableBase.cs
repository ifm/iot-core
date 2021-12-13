// ReSharper disable once CheckNamespace
namespace ifmIoTCore
{
    using System;

    /// <summary>
    /// Provides a base implementation if the disposable pattern.
    /// </summary>
    public class DisposableBase : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed) return;
            this._disposed = true;
        }
    }
}
