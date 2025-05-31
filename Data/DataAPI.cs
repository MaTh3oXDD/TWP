using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    internal class DataAPI : DataAbstractAPI
    {
        private readonly ConcurrentQueue<string> _logQueue = new();
        private readonly string _filePath = "diagnostics.txt";
        private bool _logging = true;
        private readonly Task _loggerTask;

        public DataAPI()
        {
            _loggerTask = Task.Run(LoggerLoop);
        }

        public override void LogBallState(float x, float y, float vx, float vy, float timestamp)
        {
            string entry = $"{timestamp:0.##};{x:0.##};{y:0.##};{vx:0.##};{vy:0.##}";
            _logQueue.Enqueue(entry);
        }

        private void LoggerLoop()
        {
            while (_logging || !_logQueue.IsEmpty)
            {
                while (_logQueue.TryDequeue(out var entry))
                {
                    try
                    {
                        File.AppendAllText(_filePath, entry + Environment.NewLine);
                    }
                    catch
                    {
                        Thread.Sleep(100); // Czekaj jeśli chwilowe IO error
                        _logQueue.Enqueue(entry);
                    }
                }
                Thread.Sleep(100);
            }
        }

        public override void StopLogger()
        {
            _logging = false;
            _loggerTask.Wait();
        }
    }
}
