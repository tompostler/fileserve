namespace Unlimitedinf.Fileserve.Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Serves files to the end user.
    /// </summary>
    internal sealed class FileServer : IDisposable
    {
        /// <summary>
        /// The core server handling the http requests coming in from the outside world.
        /// </summary>
        private HttpListener httpListener;
        /// <summary>
        /// The thread running the core server.
        /// </summary>
        private Thread listenerThread;
        /// <summary>
        /// A queue containing the incoming requests waiting to be processed.
        /// </summary>
        private ConcurrentQueue<HttpListenerContext> toBeProcessed;
        /// <summary>
        /// A collection of threads used to process the incoming request queue.
        /// </summary>
        private List<Thread> processingThreads;
        /// <summary>
        /// Manages the blocking/unblocking of the processing threads based on the number of requests in the queue.
        /// </summary>
        private SemaphoreSlim toBeProcessedCount;
        /// <summary>
        /// Signals to the processing threads to cancel after completing the current request.
        /// </summary>
        private CancellationTokenSource stopProcessors;

        /// <summary>
        /// Set to <see cref="Environment.ProcessorCount"/>*2 because most of the operations intended for this program
        /// should be I/O intensive.
        /// </summary>
        private static int MaxProcessorsCount => Environment.ProcessorCount * 2;

        /// <summary>
        /// Has the server been successfully stopped.
        /// </summary>
        private bool stopped = false;

        /// <summary>
        /// Ctor.
        /// </summary>
        public FileServer()
        {
            this.httpListener = new HttpListener();
            this.toBeProcessed = new ConcurrentQueue<HttpListenerContext>();
            this.toBeProcessedCount = new SemaphoreSlim(0);
            this.processingThreads = new List<Thread>(MaxProcessorsCount);
            this.stopProcessors = new CancellationTokenSource();
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            // This program would need to be run as admin without first doing:
            //  'netsh http add urlacl url=http://{domain}:{port}/ user=Everyone listen=yes'
            // Tested on Win10.
            // For linux, to run on port 80: sudo setcap 'cap_net_bind_service=+ep' /usr/local/bin/mono-sgen
            // http://stackoverflow.com/questions/2923966 and http://stackoverflow.com/questions/4019466
            const string domain = "+";
            const int port = 80;

            // Spin up the server and listener
            this.httpListener.Prefixes.Add($"http://{domain}:{port}/");
            this.listenerThread = new Thread(this.ListenerThread);
            this.httpListener.Start();
            this.listenerThread.Start();

            // Spin up the processing threads
            for (int i = 0; i < MaxProcessorsCount; i++)
            {
                Thread processor = new Thread(this.ProcessorThread);
                processor.Start();
                this.processingThreads.Add(processor);
            }
        }

        /// <summary>
        /// Stop the server and wait for the current requests to finish.
        /// </summary>
        public void Stop()
        {
            if (this.stopped)
                return;

            // Take no more requests and signal to cancel the current ones
            this.listenerThread.Abort();
            this.stopProcessors.Cancel();
            
            // Finish up the current processing jobs and then stop the listener
            // (assuming that the listener has to live until the requests are done processing)
            foreach (Thread processor in this.processingThreads)
            {
                processor.Join();
            }
            this.httpListener.Stop();

            this.stopped = true;
        }

        /// <summary>
        /// Listens for http requests and hands them off to the queue to process.
        /// </summary>
        private void ListenerThread()
        {
            while (true)
            {
                HttpListenerContext context = this.httpListener.GetContext();
                this.toBeProcessed.Enqueue(context);
                this.toBeProcessedCount.Release();
            }
        }

        /// <summary>
        /// Several of these are spun up to handle incoming requests.
        /// </summary>
        private void ProcessorThread()
        {
            while (true)
            {
                try
                {
                    // Wait for and grab a request to process
                    this.toBeProcessedCount.Wait(this.stopProcessors.Token);
                    HttpListenerContext context;
                    this.toBeProcessed.TryDequeue(out context);
                    if (context == null)
                    {
                        this.toBeProcessedCount.Release();
                        continue;
                    }

                    this.Process(context);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Actually process an http request.
        /// </summary>
        /// <param name="context"></param>
        private void Process(HttpListenerContext context)
        {
            const string data = "Here's a string to be served up for testing purposes.";
            byte[] buf = Encoding.UTF8.GetBytes(data);

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            //context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/octet-stream";
            context.Response.ContentLength64 = buf.Length;
            context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
            //context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));

            //byte[] buffer = new byte[1024 * 16];
            //int nbytes;
            //while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
            //    context.Response.OutputStream.Write(buffer, 0, nbytes);

            context.Response.OutputStream.Write(buf, 0, buf.Length);

            context.Response.OutputStream.Flush();

            context.Response.OutputStream.Close();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Stop();
                    this.httpListener.Close();
                    this.toBeProcessed = null;
                    this.toBeProcessedCount.Dispose();
                    this.processingThreads = null;
                    this.stopProcessors.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
