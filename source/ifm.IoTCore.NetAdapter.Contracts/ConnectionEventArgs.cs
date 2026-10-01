namespace ifm.IoTCore.NetAdapter.Contracts;

using System;

/// <summary>
/// Provides data for the connection event.
/// </summary>
public class ConnectionEventArgs : EventArgs
{
    /// <summary>
    /// Defines the actions that caused the event.
    /// </summary>
    public enum Types
    {
        /// <summary>A connection was connected.</summary>
        Connected,

        /// <summary>A connection was disconnected.</summary>
        Disconnected
    }

    /// <summary>
    /// The type of the connection event.
    /// </summary>
    public Types Type { get; }

    /// <summary>
    /// The identifier of the event sender.
    /// </summary>
    public string SenderId { get; }

    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    /// <param name="type">The type of the connection event.</param>
    /// <param name="senderId">The identifier of the event sender.</param>
    public ConnectionEventArgs(Types type, string senderId = null)
    {
        Type = type;
        SenderId = senderId;
    }
}