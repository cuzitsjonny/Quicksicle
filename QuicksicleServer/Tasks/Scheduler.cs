using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using Quicksicle.Threading;

namespace Quicksicle.Tasks
{
    /// <summary>
    /// The scheduler of the Quicksicle server.
    /// It has a real time tick rate of 100TPS.
    /// (One tick every 10ms)
    /// </summary>
    public class Scheduler : BaseThread
    {
        // This is the task queue. Tasks wait in here to be processed.
        private Queue<Task> taskQueue;

        /// <summary>
        /// Constructs a new Scheduler object with an empty task queue.
        /// </summary>
        public Scheduler()
        {
            this.taskQueue = new Queue<Task>();
        }

        /// <summary>
        /// The scheduler is running a loop that checks if the next tick is reached on each iteration.
        /// When the next tick is reached, all tasks in the task queue are either invoked, not invoked, or requeued
        /// based on their IsCancelled and TicksToInvocation properties.
        /// </summary>
        public override void Run()
        {
            List<Task> tasksToBeRescheduled = new List<Task>();
            Stopwatch timer = Stopwatch.StartNew();
            double nextTick = 10.0;

            while (!terminationRequested)
            {
                Monitor.Enter(taskQueue);

                if (timer.Elapsed.TotalMilliseconds >= nextTick)
                {
                    int taskCount = taskQueue.Count;

                    if (taskCount > 0)
                    {
                        for (int i = 0; i < taskCount; i++)
                        {
                            Task task = taskQueue.Dequeue();

                            if (!task.IsCancelled)
                            {
                                if (task.TicksToInvocation == 0)
                                {
                                    if (task.IsAsync)
                                    {
                                        new Thread(new ThreadStart(task.Action.Invoke)).Start();
                                    }
                                    else
                                    {
                                        task.Action.Invoke();
                                    }

                                    if (task is PeriodicTask)
                                    {
                                        PeriodicTask periodicTask = (PeriodicTask)task;

                                        periodicTask.TicksToInvocation = periodicTask.Interval - 1;

                                        tasksToBeRescheduled.Add(periodicTask);
                                    }
                                }
                                else
                                {
                                    task.TicksToInvocation = task.TicksToInvocation - 1;

                                    tasksToBeRescheduled.Add(task);
                                }
                            }
                        }

                        foreach (Task task in tasksToBeRescheduled)
                        {
                            taskQueue.Enqueue(task);
                        }

                        tasksToBeRescheduled.Clear();
                    }

                    nextTick += 10.0;
                }

                Monitor.Exit(taskQueue);
            }

            timer.Stop();
        }

        /// <summary>
        /// Enqueues a task for synchronous execution on the next tick.
        /// </summary>
        /// <param name="action">The action that will be invoked on the next tick.</param>
        public TaskHandle RunTask(Action action)
        {
            TaskHandle handle;

            Monitor.Enter(taskQueue);

            Task task = new Task(action, false, 0);

            taskQueue.Enqueue(task);
            handle = new TaskHandle(task);

            Monitor.Exit(taskQueue);

            return handle;
        }

        /// <summary>
        /// Enqueues a task for delayed synchronous execution.
        /// </summary>
        /// <param name="action">The action that will be invoked when the delay is over.</param>
        /// <param name="delay">The delay in ticks.</param>
        public TaskHandle RunDelayedTask(Action action, int delay)
        {
            TaskHandle handle;

            Monitor.Enter(taskQueue);

            Task task = new Task(action, false, delay);

            taskQueue.Enqueue(task);
            handle = new TaskHandle(task);

            Monitor.Exit(taskQueue);

            return handle;
        }

        /// <summary>
        /// Enqueues a task for periodic synchronous execution.
        /// </summary>
        /// <param name="action">The action that will be invoked when the delay is over and then everytime the interval is over.</param>
        /// <param name="delay">The delay in ticks.</param>
        /// <param name="interval">The interval in ticks.</param>
        public TaskHandle RunPeriodicTask(Action action, int delay, int interval)
        {
            TaskHandle handle;

            Monitor.Enter(taskQueue);

            Task task = new PeriodicTask(action, false, interval, delay);

            taskQueue.Enqueue(task);
            handle = new TaskHandle(task);

            Monitor.Exit(taskQueue);

            return handle;
        }

        /// <summary>
        /// Enqueues a task for asynchronous execution on the next tick.
        /// </summary>
        /// <param name="action">The action that will be invoked on the next tick.</param>
        public TaskHandle RunTaskAsync(Action action)
        {
            TaskHandle handle;

            Monitor.Enter(taskQueue);

            Task task = new Task(action, true, 0);

            taskQueue.Enqueue(task);
            handle = new TaskHandle(task);

            Monitor.Exit(taskQueue);

            return handle;
        }

        /// <summary>
        /// Enqueues a task for delayed asynchronous execution.
        /// </summary>
        /// <param name="action">The action that will be invoked when the delay is over.</param>
        /// <param name="delay">The delay in ticks.</param>
        public TaskHandle RunDelayedTaskAsync(Action action, int delay)
        {
            TaskHandle handle;

            Monitor.Enter(taskQueue);

            Task task = new Task(action, true, delay);

            taskQueue.Enqueue(task);
            handle = new TaskHandle(task);

            Monitor.Exit(taskQueue);

            return handle;
        }

        /// <summary>
        /// Enqueues a task for periodic asynchronous execution.
        /// </summary>
        /// <param name="action">The action that will be invoked when the delay is over and then everytime the interval is over.</param>
        /// <param name="delay">The delay in ticks.</param>
        /// <param name="interval">The interval in ticks.</param>
        public TaskHandle RunPeriodicTaskAsync(Action action, int delay, int interval)
        {
            TaskHandle handle;

            Monitor.Enter(taskQueue);

            Task task = new PeriodicTask(action, true, interval, delay);

            taskQueue.Enqueue(task);
            handle = new TaskHandle(task);

            Monitor.Exit(taskQueue);

            return handle;
        }
    }
}
