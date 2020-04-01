using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksicle.Events
{
    public class EventSubscription<T> : BaseEventSubscription
    {
        public EventSubscription(Action<T> action) : base()
        {
            this.Action = action;
        }

        public Action<T> Action
        {
            get;
        }
    }
}
