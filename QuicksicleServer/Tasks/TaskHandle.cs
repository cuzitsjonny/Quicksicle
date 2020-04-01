using System;

namespace Quicksicle.Tasks
{
    /// <summary>
    /// Represents a way to cancel
    /// a task without making the
    /// inner workings of the
    /// Scheduler class accessible.
    /// </summary>
    public class TaskHandle
    {
        // The task this handle is attached to.
        private Task task;

        /// <summary>
        /// Constructs a new TaskHandle.
        /// This is only used inside the
        /// Scheduler class.
        /// </summary>
        public TaskHandle(Task task)
        {
            this.task = task;
        }

        /// <summary>
        /// Checks if the task that is attached
        /// to this handle is cancelled.
        /// </summary>
        public bool IsCancelled => task.IsCancelled;

        /// <summary>
        /// Cancels the task that is attached
        /// to this handle.
        /// Does nothing if the task is
        /// already cancelled.
        /// </summary>
        public void CancelTask()
        {
            task.IsCancelled = true;
        }
    }
}
