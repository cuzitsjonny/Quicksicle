using System;

namespace Quicksicle.Tasks
{
    public class Task
    {
        public Task(Action action, bool isAsync, int ticksToInvocation)
        {
            this.Action = action;
            this.IsCancelled = false;
            this.IsAsync = isAsync;
            this.TicksToInvocation = ticksToInvocation;
        }

        public Action Action
        {
            get;
        }

        public bool IsCancelled
        {
            get;
            set;
        }

        public bool IsAsync
        {
            get;
        }

        public int TicksToInvocation
        {
            get;
            set;
        }
    }
}
