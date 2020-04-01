using System;
using System.Collections.Generic;
using System.Threading;
using Quicksicle.Tasks;

namespace Quicksicle.Events
{
    /// <summary>
    /// The event manager of the Quicksicle server.
    /// It implements the publish/subscribe pattern
    /// for events.
    /// </summary>
    public class EventManager
    {
        // The subscription map. In here all subscriptions are saved and
        // indexed by their type.
        private Dictionary<Type, BaseEventSubscriptionList> eventSubscriptionLists;

        /// <summary>
        /// Constructs a new EventManager.
        /// </summary>
        public EventManager()
        {
            this.eventSubscriptionLists = new Dictionary<Type, BaseEventSubscriptionList>();
        }

        /// <summary>
        /// Publishes an event. Never call this outside of the main scheduler thread.
        /// </summary>
        /// <param name="e">The event that will be published.</param>
        public void Publish<T>(T e) where T : BaseEvent
        {
            Monitor.Enter(eventSubscriptionLists);

            if (eventSubscriptionLists.ContainsKey(typeof(T)))
            {
                EventSubscriptionList<T> eventSubscriptionList = (EventSubscriptionList<T>)eventSubscriptionLists[typeof(T)];

                eventSubscriptionList.ForEach(

                    (EventSubscription<T> eventSubscription) =>
                    {
                        eventSubscription.Action.Invoke(e);
                    }

                );
            }

            Monitor.Exit(eventSubscriptionLists);
        }

        /// <summary>
        /// Saves a subscription to a certain event.
        /// </summary>
        /// <param name="action">The action that will be executed synchronously when an event of the specified type is published.</param>
        public EventSubscriptionHandle Subscribe<T>(Action<T> action) where T : BaseEvent
        {
            EventSubscriptionHandle handle;

            Monitor.Enter(eventSubscriptionLists);

            EventSubscription<T> eventSubscription = new EventSubscription<T>(action);
            EventSubscriptionList<T> eventSubscriptionList;

            handle = new EventSubscriptionHandle(eventSubscription);

            if (eventSubscriptionLists.ContainsKey(typeof(T)))
            {
                eventSubscriptionList = (EventSubscriptionList<T>)eventSubscriptionLists[typeof(T)];
            }
            else
            {
                eventSubscriptionList = new EventSubscriptionList<T>();

                eventSubscriptionLists[typeof(T)] = eventSubscriptionList;
            }

            eventSubscriptionList.Add(eventSubscription);

            Monitor.Exit(eventSubscriptionLists);

            return handle;
        }
    }
}
