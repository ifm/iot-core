namespace ifm.IoTCore.PersistenceManager.Contracts;

using System.Collections.Generic;
using Common.Variant;

/// <summary>
/// Provides functionality to interact with the persistence manager.
/// </summary>
public interface IPersistenceManager
{
    /// <summary>
    /// Represents a persisted service call.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of the class.
    /// </remarks>
    /// <param name="serviceAddress">The address of the called service.</param>
    /// <param name="serviceData">The data of the called service.</param>
    public class ServiceCallInfo(string serviceAddress, Variant serviceData)
    {
        /// <summary>
        /// The address of the called service.
        /// </summary>
        public string ServiceAddress { get; } = serviceAddress;

        /// <summary>
        /// The incoming data of the called service.
        /// </summary>
        public Variant ServiceData { get; } = serviceData;
    }

    /// <summary>
    /// Locks the persistence manager.
    /// </summary>
    void Lock();

    /// <summary>
    /// Unlocks the persistence manager.
    /// </summary>
    void Unlock();

    /// <summary>
    /// Gets the persisted service calls.
    /// </summary>
    IReadOnlyList<ServiceCallInfo> PersistedItems { get; }

    /// <summary>
    /// Persists a service call.
    /// </summary>
    /// <param name="serviceAddress">The address of the called service.</param>
    /// <param name="serviceData">The incoming data of the called service.</param>
    /// <param name="flush">If true the persisted items are written to disk; otherwise not.</param>
    void Persist(string serviceAddress, Variant serviceData, bool flush = true);

    /// <summary>
    /// Removes a persisted service call.
    /// </summary>
    /// <param name="item">The service call info.</param>
    /// <param name="flush">If true the persisted items are written to disk; otherwise not.</param>
    void Remove(ServiceCallInfo item, bool flush = true);

    /// <summary>
    /// The persisted items are written to disk.
    /// </summary>
    void Flush();

    /// <summary>
    /// Returns the persisted service calls against the IoTCore.
    /// </summary>
    void Restore();
}