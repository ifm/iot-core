namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using EventArguments;

    /// <summary>
    /// Provides functionality to interact with an event element
    /// </summary>
    public interface IEventElement : IBaseElement
    {
        /// <summary>
        /// Locks access to subscriptions
        /// </summary>
        ReaderWriterLockSlim SubscriptionsLock { get; }

        /// <summary>
        /// Gets the subscriptions of the element
        /// </summary>
        IEnumerable<Subscription> Subscriptions { get; }

        /// <summary>
        /// Gets the subscribe service element
        /// </summary>
        IServiceElement SubscribeServiceElement { get; }

        /// <summary>
        /// Gets the unsubscribe service element
        /// </summary>
        IServiceElement UnsubscribeServiceElement { get; }

        /// <summary>
        /// Adds a subscription to a local callback method to the element
        /// </summary>
        /// <param name="callbackFunc">The local callback method</param>
        /// <returns>The subscription id</returns>
        void Subscribe(Action<IEventElement> callbackFunc);

        /// <summary>
        /// Removes a subscription from the element
        /// </summary>
        /// <param name="callbackFunc">The local callback method</param>
        void Unsubscribe(Action<IEventElement> callbackFunc);

        /// <summary>
        /// Checks if subscription exists
        /// </summary>
        /// <param name="callbackFunc">The local callback method</param>
        /// <returns>true, if the subscription exists; otherwise false</returns>
        bool HasSubscription(Action<IEventElement> callbackFunc);

        /// <summary>
        /// Raises the event
        /// </summary>
        void RaiseEvent();

        /// <summary>
        /// The event handler that is called when the event is raised
        /// </summary>
        public Action<IEventElement, SubscriptionEventArgs> RaiseEventFunc { get; set; }
    }
}
