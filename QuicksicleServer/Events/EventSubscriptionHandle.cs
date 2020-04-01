using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksicle.Events
{
    public class EventSubscriptionHandle
    {
        private BaseEventSubscription eventSubscription;

        public EventSubscriptionHandle(BaseEventSubscription eventSubscription)
        {
            this.eventSubscription = eventSubscription;
        }

        public bool IsUnsubscribed => eventSubscription.IsUnsubscribed;

        public void Unsubscribe()
        {
            eventSubscription.IsUnsubscribed = true;
        }
    }
}
