using System;
using Newtonsoft.Json.Linq;

namespace ifmIoTCore.Messages
{
    /// <summary>
    /// Provides functionality to interact with the message handler
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
        /// <param name="cid">The cid for the request</param>
        /// <param name="address">The target service address for the request</param>
        /// <param name="data">The data for the request</param>
        /// <param name="reply">The reply address for the request's response</param>
        /// <returns>The response message</returns>
        ResponseMessage HandleRequest(int cid, string address, JToken data = null, string reply = null);

        /// <summary>
        /// Handles a request on a service element in the IoTCore tree
        /// </summary>
        /// <param name="message">The request message</param>
        /// <returns>The response message</returns>
        ResponseMessage HandleRequest(RequestMessage message);

        /// <summary>
        /// Handles an event on a service element in the IoTCore tree
        /// </summary>
        /// <param name="message">The event message</param>
        void HandleEvent(EventMessage message);
    }
}