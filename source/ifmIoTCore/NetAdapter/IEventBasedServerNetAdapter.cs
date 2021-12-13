namespace ifmIoTCore.NetAdapter
{
    using System;
    using Messages;

    /// <summary>
    /// Declares an interface for an event based servernetadapter.
    /// </summary>
    public interface IEventBasedServerNetAdapter : IServerNetAdapter
    {
        /// <summary>
        /// This event will be fired in case the servernetadapter has received a request.
        /// </summary>
        /// <remarks>The response of the received request can be given in the <seealso cref="RequestMessageEventArgs.Response"/> field.</remarks>
        event EventHandler<RequestMessageEventArgs> RequestMessageReceived;

        /// <summary>
        /// This event will be fired in case the servernetadapter has received an event.
        /// </summary>
        event EventHandler<EventMessageEventArgs> EventMessageReceived;
    }
}