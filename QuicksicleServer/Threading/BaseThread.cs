using System;
using System.Threading;

namespace Quicksicle.Threading
{
    /// <summary>
    /// A base class for classes that ARE a thread or that are using internal threads for management.
    /// </summary>
    public abstract class BaseThread
    {
        private Thread thread;

        // This is the most important thing about this class.
        // Implementations of the class use this protected bool in their overrides of Run
        // to find out if the thread has been terminated by something running
        // in the outside world.
        protected bool terminationRequested;

        /// <summary>
        /// Constructs a new BaseThread and initializes the member thread with the implementation's override of Run.
        /// </summary>
        protected BaseThread()
        {
            this.thread = new Thread(new ThreadStart(Run));
        }

        /// <summary>
        /// Points to the member thread's IsAlive property.
        /// </summary>
        public bool IsAlive => thread.IsAlive;

        /// <summary>
        /// Starts the member thread.
        /// </summary>
        public void Start()
        {
            terminationRequested = false;

            thread.Start();
        }

        /// <summary>
        /// Tells the member thread that it's time to shut down.
        /// </summary>
        public void Terminate()
        {
            terminationRequested = true;

            Join();
        }

        /// <summary>
        /// Points to the member thread's Join method.
        /// </summary>
        public void Join()
        {
            thread.Join();
        }

        /// <summary>
        /// This has to be overriden by implementations of BaseThread.
        /// When overrding this method it's very important to monitor
        /// the state of terminationRequested in some way.
        /// The majority of implementations will run a loop in here
        /// anyway, so it would be best to put the check into the
        /// head of the loop.
        /// </summary>
        public abstract void Run();
    }
}
