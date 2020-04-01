using System;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using Quicksicle.Threading;

namespace Quicksicle.Logging
{
    /// <summary>
    /// A thread-safe console and file writer.
    /// </summary>
    public class Logger : BaseThread
    {
        private string directoryPath;
        private string latestFilePath;
        private string dateTimeFormat;
        private int fileSplitPeriod;
        private DateTime nextSplit;
        private ConcurrentQueue<string> messageQueue;

        /// <summary>
        /// Constructs a new Logger object.
        /// </summary>
        /// <param name="directoryPath">The directory this Logger will put files into.</param>
        /// <param name="dateTimeFormat">The format this Logger will use for console and file log entries. Also split file names will be generated using the format.</param>
        /// <param name="fileSplitPeriod">The period in minutes the Logger will keep writing to the same file. When the period is over, the next log entry will be written to a new file. The old one will be renamed to the current date time.</param>
        public Logger(string directoryPath, string dateTimeFormat, int fileSplitPeriod)
        {
            this.directoryPath = directoryPath;
            this.latestFilePath = Path.Combine(directoryPath, "latest.log");
            this.dateTimeFormat = dateTimeFormat;
            this.fileSplitPeriod = fileSplitPeriod;
            this.messageQueue = new ConcurrentQueue<string>();
        }

        /// <summary>
        /// Renames the latest log file to the current date time.
        /// </summary>
        /// <param name="fileSplitPeriod">The current date time.</param>
        private void Split(DateTime now)
        {
            if (File.Exists(latestFilePath))
            {
                File.Move(latestFilePath, GetSplitFilePath(now));
            }

            nextSplit = now.AddMinutes(fileSplitPeriod);
        }

        /// <summary>
        /// Constructs the path for a new split file.
        /// </summary>
        /// <param name="now">The current date time.</param>
        private string GetSplitFilePath(DateTime now)
        {
            return Path.Combine(directoryPath, now.ToString(dateTimeFormat) + ".log");
        }

        /// <summary>
        /// Overrides the Run method of the BaseThread class.
        /// </summary>
        public override void Run()
        {
            DateTime now = DateTime.Now;

            // If the logs directory doesn't exist yet, we'll create it.
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // If the latest log file is still in the directory, we'll just split it.
            Split(now);

            while (!terminationRequested || !messageQueue.IsEmpty)
            {
                string message;

                // If there is something in the queue, we'll write it to the console and the latest log file. If not, it's time to sleep... for 10ms.
                if (messageQueue.TryDequeue(out message))
                {
                    now = DateTime.Now;

                    // If it's time to split, split. Easy.
                    if (now >= nextSplit)
                    {
                        Split(now);
                    }

                    string timestamp = now.ToString(dateTimeFormat);
                    string entry = "[" + timestamp + "] " + message;

                    Console.WriteLine(entry);

                    File.AppendAllText(latestFilePath, entry + Environment.NewLine);
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        /// <summary>
        /// Adds a message to the Logger's message queue. It will be printed to the console and written to a file concurrently.
        /// </summary>
        /// <param name="message">The message to be added to the Logger's message queue.</param>
        public void Log(String message)
        {
            messageQueue.Enqueue(message);
        }
    }
}