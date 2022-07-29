using System.Collections.Generic;

namespace ifmIoTCore.Elements
{
    using System;

    /// <summary>
    /// The subscription class
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// The id which identifies the subscription
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The url to which the IoTCore is sending events
        /// </summary>
        public string Callback { get; }

        /// <summary>
        /// The callback handler to which the IoTCore is sending events
        /// </summary>
        public readonly Action<IEventElement> CallbackFunc;

        /// <summary>
        /// List of data element addresses, whose values are sent with the event
        /// </summary>
        public List<string> DataToSend { get; }

        /// <summary>
        ///  If true the subscription is persisted; otherwise not
        /// </summary>
        public bool Persist { get; }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="id">The id which identifies the subscription</param>
        /// <param name="callback">The callback to which the IoTCore is sending events</param>
        /// <param name="callbackFunc">The callback handler to which the IoTCore is sending events</param>
        /// <param name="dataToSend">List of data element addresses, whose values are sent with the event</param>
        /// <param name="persist">If true the subscription is persisted; otherwise not</param>
        public Subscription(int id, string callback, Action<IEventElement> callbackFunc, List<string> dataToSend, bool persist)
        {
            Id = id;
            Callback = callback;
            CallbackFunc = callbackFunc;
            DataToSend = dataToSend;
            Persist = persist;
        }
    }
}