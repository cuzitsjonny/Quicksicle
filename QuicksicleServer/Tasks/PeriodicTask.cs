using System;

namespace Quicksicle.Tasks
{
    public class PeriodicTask : Task
    {
        public PeriodicTask(Action action, bool isAsync, int interval, int ticksToInvocation) : base(action, isAsync, ticksToInvocation)
        {
            this.Interval = interval;
        }

        public int Interval
        {
            get;
            set;
        }
    }
}
