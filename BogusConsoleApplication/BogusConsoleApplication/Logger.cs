using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace BogusConsoleApplication
{
    public class Logger : IDisposable
    {
        private readonly object _writeLock = new object();

        private Stopwatch _stopwatch;
        private bool _disposed;

        public void Open()
        {
            _stopwatch = Stopwatch.StartNew(); // more accurate than DateTime for values less than 1s.
            Log("[Logger] File open");
        }

        public void Log(string msg)
        {
            Debug.WriteLine(msg);
            lock (_writeLock)
            {
                Console.WriteLine("{0,9:F3} [{1,3}] : {2}", _stopwatch.Elapsed.TotalSeconds, Thread.CurrentThread.ManagedThreadId, msg);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }
            }

            _disposed = true;
        }
    }
}