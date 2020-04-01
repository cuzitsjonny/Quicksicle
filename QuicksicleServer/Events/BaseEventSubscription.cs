using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksicle.Events
{
    public abstract class BaseEventSubscription
    {
        protected BaseEventSubscription()
        {
            this.IsUnsubscribed = false;
        }

        public bool IsUnsubscribed
        {
            get;
            set;
        }

        public void Unsubscribe()
        {
            IsUnsubscribed = true;
        }
    }
}
