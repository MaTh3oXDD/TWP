using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Utils
{
    public class DiagnosticsLogger : IDiagnosticsLogger, IDisposable
    {
        private readonly BlockingCollection<string> _queue = new();
        private readonly string _filePath;
        private readonly Task _backgroundTask;
        private readonly CancellationTokenSource _cts = new();

        public DiagnosticsLogger(string filePath)
        {
            _filePath = filePath;
            _backgroundTask = Task.Run(ProcessQueue);
        }

        public void Log(string message)
        {
            _queue.Add(message);
        }

        private void ProcessQueue()
        {
            FileStream? fileStream = null;
            StreamWriter? writer = null;

            try
            {
                while (!fileStream?.CanWrite ?? true)
                {
                    try
                    {
                        fileStream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        writer = new StreamWriter(fileStream, Encoding.ASCII);
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(100);
                    }
                }

                foreach (var line in _queue.GetConsumingEnumerable(_cts.Token))
                {
                    bool written = false;
                    while (!written)
                    {
                        try
                        {
                            writer.WriteLine(line);
                            writer.Flush();
                            written = true;
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(10);
                        }
                    }
                }
            }
            finally
            {
                writer?.Dispose();
                fileStream?.Dispose();
            }
        }

        public void Dispose()
        {
            _queue.CompleteAdding();
            _backgroundTask.Wait();
        }
    }
}
