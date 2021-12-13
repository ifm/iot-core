namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using ServiceData.Requests;
    using ServiceData.Responses;

    /// <summary>
    /// Provides functionality to interact with an event element
    /// </summary>
    public interface IEventElement : IBaseElement
    {
        /// <summary>
        /// Gets the subscriptions of the element
        /// </summary>
        IDictionary<int, Subscription> Subscriptions { get; }

        /// <summary>
        /// Locks access to subscriptions
        /// </summary>
        ReaderWriterLockSlim SubscriptionsLock { get; }

        /// <summary>
        /// Adds a subscription to the element
        /// </summary>
        /// <param name="data">The subscription to add</param>
        /// <param name="cid">The context id</param>
        SubscribeResponseServiceData Subscribe(SubscribeRequestServiceData data, int? cid = null);

        /// <summary>
        /// Adds a subscription to a local callback method to the element
        /// </summary>
        /// <param name="callbackFunc">The local callback method</param>
        int Subscribe(Action<IEventElement> callbackFunc);

        /// <summary>
        /// Removes a subscription from the element
        /// </summary>
        /// <param name="data">The subscription to remove</param>
        /// <param name="cid">The context id</param>
        void Unsubscribe(UnsubscribeRequestServiceData data, int? cid = null);

        /// <summary>
        /// Removes a subscription from the element
        /// </summary>
        /// <param name="subscriptionId">The subscription to remove</param>
        void Unsubscribe(int subscriptionId);

        /// <summary>
        /// Removes a subscription from the element
        /// </summary>
        /// <param name="callbackFunc">The local callback method</param>
        void Unsubscribe(Action<IEventElement> callbackFunc);

        /// <summary>
        /// Raises the event
        /// </summary>
        void RaiseEvent();
    }
}
