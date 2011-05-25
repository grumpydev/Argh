namespace Argh
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using HttpClient;

    /// <summary>
    /// Basic http tester/benchmarker/stress tool
    /// Uses the async http client to throw simultaneous requests at a service and
    /// monitors the number of errors, execution time, max. number of concurrent connections
    /// and requsests/second.
    /// </summary>
    public class HttpTester
    {
        private int iterations;

        private RequestConfiguration requestConfiguration;

        private AsyncHttpClient httpClient;

        private int totalExecuted;

        private int errored;

        private int inProgress;

        private object peakInProgressLock = new object();
        private int peakInProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTester"/> class.
        /// </summary>
        /// <param name="iterations">Number of iterations to run</param>
        /// <param name="requestConfiguration">The request configuration to run</param>
        /// <param name="httpClient">The async http client to use</param>
        public HttpTester(int iterations, RequestConfiguration requestConfiguration, AsyncHttpClient httpClient)
        {
            this.iterations = iterations;
            this.requestConfiguration = requestConfiguration;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Execute the test
        /// </summary>
        /// <returns>Test results upon completion</returns>
        public HttpTesterResults Execute()
        {
            this.ResetCounters();

            var counter = new CountdownEvent(this.iterations);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (var current = 0; current < this.iterations; current++)
            {
                var value = Interlocked.Increment(ref this.inProgress);

                lock (this.peakInProgressLock)
                {
                    if (value > this.peakInProgress)
                    {
                        this.peakInProgress = value;
                    }
                }

                this.httpClient.Execute(
                    this.requestConfiguration,
                    r =>
                        {
                            //using (var reader = new StreamReader(r.GetResponseStream()))
                            //{
                            //    var body = reader.ReadToEnd();
                            //}

                            Interlocked.Increment(ref this.totalExecuted);
                            Interlocked.Decrement(ref this.inProgress);
                            counter.Signal();
                        },
                    e =>
                        {
                            Interlocked.Increment(ref this.totalExecuted);
                            Interlocked.Increment(ref this.errored);
                            Interlocked.Decrement(ref this.inProgress);
                            counter.Signal();
                        });
            }

            counter.Wait();

            stopwatch.Stop();

            return new HttpTesterResults(this.totalExecuted, this.errored, this.peakInProgress, stopwatch.Elapsed);
        }

        private void ResetCounters()
        {
            this.totalExecuted = 0;
            this.errored = 0;
            this.inProgress = 0;
            this.peakInProgress = 0;
        }
    }
}