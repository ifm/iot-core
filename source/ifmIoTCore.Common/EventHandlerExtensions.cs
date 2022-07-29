namespace ifmIoTCore.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EventHandlerExtensions
    {
        public static void Raise(this EventHandler eventHandler, object sender, bool raiseAggregateException = false)
        {
            if (eventHandler == null) return;

            var caughtExceptions = new List<Exception>();
            var eventHandlerDelegates = eventHandler.GetInvocationList();

            foreach (var item in eventHandlerDelegates.Cast<EventHandler>())
            {
                try
                {
                    item(sender,EventArgs.Empty);
                }
                catch (Exception e)
                {
                    caughtExceptions.Add(e);
                }
            }

            if (raiseAggregateException && caughtExceptions.Any())
            {
                throw new AggregateException(caughtExceptions);
            }
        }

        /// <summary>
        /// Safely raises an event. If any of the delegates in the invocationlist throws an exception, the other delegates will still be called.
        /// </summary>
        /// <param name="eventHandler">The eventhandler for the event that should be raised.</param>
        /// <param name="sender">The sender of the event.</param>
        /// /// <param name="eventArgs">The eventargs.</param>
        /// <param name="raiseAggregateException">When true, an aggregate exception will be thrown, when any of the delegates in the invocationlist throws an exception. When false, no exception will be thrown.</param>
        public static void Raise<T>(this EventHandler eventHandler, object sender, T eventArgs, bool raiseAggregateException = false) where T : EventArgs
        {
            if (eventHandler == null) return;

            var caughtExceptions = new List<Exception>();
            var eventHandlerDelegates = eventHandler.GetInvocationList();

            foreach (var item in eventHandlerDelegates.Cast<EventHandler>())
            {
                try
                {
                    item(sender, eventArgs);
                }
                catch (Exception e)
                {
                    caughtExceptions.Add(e);
                }
            }

            if (raiseAggregateException && caughtExceptions.Any())
            {
                throw new AggregateException(caughtExceptions);
            }
        }

        /// <summary>
        /// Safely raises an event. If any of the delegates in the invocationlist throws an exception, the other delegates will still be called.
        /// </summary>
        /// <typeparam name="T">The type of the eventargs.</typeparam>
        /// <param name="eventHandler">The eventhandler for the event that should be raised.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The eventargs.</param>
        /// <param name="raiseAggregateException">When true, an aggregate exception will be thrown, when any of the delegates in the invocationlist throws an exception. When false, no exception will be thrown.</param>
        public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T eventArgs, bool raiseAggregateException = false)
        {
            if (eventHandler == null) return;

            var caughtExceptions = new List<Exception>();
            var eventHandlerDelegates = eventHandler.GetInvocationList();

            foreach (var item in eventHandlerDelegates.Cast<EventHandler<T>>())
            {
                try
                {
                    item(sender, eventArgs);
                }
                catch (Exception e)
                {
                    caughtExceptions.Add(e);
                }
            }

            if (raiseAggregateException && caughtExceptions.Any())
            {
                throw new AggregateException(caughtExceptions);
            }
        }
    }
}
