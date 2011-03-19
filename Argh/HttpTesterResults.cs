namespace Argh
{
    using System;

    /// <summary>
    /// Results of a test run
    /// </summary>
    public class HttpTesterResults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTesterResults"/> class.
        /// </summary>
        /// <param name="totalExecuted">The total number of iterations</param>
        /// <param name="errored">The number of iterations that threw errors</param>
        /// <param name="peakSimultaneous">Maximum amount of concurrent connections</param>
        /// <param name="executionTime">Total execution time</param>
        public HttpTesterResults(int totalExecuted, int errored, int peakSimultaneous, TimeSpan executionTime)
        {
            this.TotalExecuted = totalExecuted;
            this.Errored = errored;
            this.PeakSimultaneous = peakSimultaneous;
            this.ExecutionTime = executionTime;

            this.RequestsPerSecond = this.TotalExecuted / this.ExecutionTime.TotalSeconds;
        }

        /// <summary>
        /// Gets the total number of iterations
        /// </summary>
        public int TotalExecuted { get; private set; }

        /// <summary>
        /// Gets the number of iterations that threw errors
        /// </summary>
        public int Errored { get; private set; }

        /// <summary>
        /// Gets a value indicating what the maximum amount of concurrent connections was
        /// </summary>
        public int PeakSimultaneous { get; private set; }

        /// <summary>
        /// Gets the total execution time
        /// </summary>
        public TimeSpan ExecutionTime { get; private set; }

        /// <summary>
        /// Gets the number of requests processed per second
        /// </summary>
        public double RequestsPerSecond { get; private set; }
    }
}