using System;
using System.Collections.Generic;

namespace Quicksicle.Events
{
    public class EventSubscriptionList<T> : BaseEventSubscriptionList
    {
        private List<EventSubscription<T>> eventSubscriptions;

        public EventSubscriptionList()
        {
            this.eventSubscriptions = new List<EventSubscription<T>>();
        }

        private void Clean()
        {
            for (int i = eventSubscriptions.Count - 1; i >= 0; i--)
            {
                EventSubscription<T> eventSubscription = eventSubscriptions[i];

                if (eventSubscription.IsUnsubscribed)
                {
                    eventSubscriptions.RemoveAt(i);
                }
            }
        }

        public void Add(EventSubscription<T> eventSubscription)
        {
            eventSubscriptions.Add(eventSubscription);
        }

        public void ForEach(Action<EventSubscription<T>> action)
        {
            Clean();

            foreach (EventSubscription<T> eventSubscription in eventSubscriptions)
            {
                action.Invoke(eventSubscription);
            }
        }
    }
}
