namespace ifmIoTCore.Messages
{
    using System;

    /// <summary>
    /// Defines the interface of a class that can handle requests and events.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// This event gets raised, before the iotcore handles an incoming message.
        /// </summary>
        event EventHandler<RequestMessageEventArgs> RequestMessageReceived;

        /// <summary>
        /// This event gets raised, after the iotcore handled an incoming event.
        /// </summary>
        event EventHandler<EventMessageEventArgs> EventMessageReceived;

        /// <summary>
        /// This event gets raised, after the iotcore handled an incoming message.
        /// </summary>
        event EventHandler<RequestMessageEventArgs> RequestMessageResponded;

        /// <summary>
        /// Handles a request on a service element in the IoTCore tree
        /// </summary>
        /// <param name="message">The request message</param>
        /// <returns>The response message</returns>
        Message HandleRequest(Message message);

        /// <summary>
        /// Handles an event on a service element in the IoTCore tree
        /// </summary>
        /// <param name="message">The event message</param>
        void HandleEvent(Message message);
    }
}