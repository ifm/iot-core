namespace ifm.Common;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides a base class implementation of the disposable pattern.
/// </summary>
public class DisposableBase : IDisposable
{
    /// <summary>
    /// Gets whether class instance has been disposed.
    /// Classes that implement the IDisposable interface should raise an ObjectDisposedException, if a disposed instance of the class is accessed.
    /// </summary>
    protected bool IsDisposed { get; private set; }

    /// <summary>
    /// Disposes a class instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        // This object is cleaned up by the Dispose method. Therefore, call GC.SuppressFinalize to take this
        // object off the finalization queue and prevent finalization code for this object from executing a second time.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs any necessary final clean-up when a class instance is being collected by the garbage collector.
    /// This finalizer will run only if the Dispose method does not get called, because Dispose will take if off the finalization queue
    /// Do not provide finalizer in types derived from this class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    ~DisposableBase()
    {
        // Set disposing to false, because if Dispose was not called, the finalizer of the managed objects will also be called
        // So clean-up only unmanaged resources
        Dispose(false);
    }

    /// <summary>
    /// Disposes a class instance.
    /// If disposing equals true, the method has been called directly or indirectly by user code. Managed and unmanaged resources can be disposed.
    /// If disposing equals false, the method has been called by the runtime from inside the finalizer and no other object should be referenced. Only unmanaged resources can be disposed.
    /// Overrides need to implement their clean-up code accordingly and call the base class implementation.
    /// </summary>
    /// <param name="disposing">true, if called by user code; false, if called by runtime from inside the finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed) return;

        // Release all unmanaged resources

        if (disposing)
        {
            // Release all managed resources
        }

        // Set large fields to null

        IsDisposed = true;

        // Call the base class implementation
    }
}