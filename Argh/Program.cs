namespace Argh
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using HttpClient;

    class Program
    {
        static void Main(string[] args)
        {
            var stringConfig = new RequestConfiguration()
                {
                    Method = HttpMethod.Get,
                    Url = "http://localhost:40965/test",
                    NoCache = true,
                };

            var sparkConfig = new RequestConfiguration()
            {
                Method = HttpMethod.Get,
                Url = "http://localhost:40965/spark",
                NoCache = true,
            };

            var results = Run(new[] { stringConfig, sparkConfig });

            Console.WriteLine("\nResults:\n");
            foreach (var result in results)
            {
                Console.WriteLine(result.Item1.Name);
                Console.WriteLine("Completed {0} iterations, {1} errors, {2} peak simultaneous, {3:0.00} seconds, {4:0.00} req/sec", result.Item2.TotalExecuted, result.Item2.Errored, result.Item2.PeakSimultaneous, result.Item2.ExecutionTime.TotalSeconds, result.Item2.TotalExecuted / result.Item2.ExecutionTime.TotalSeconds);
                Console.WriteLine();
            }

            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();
        }

        private static IEnumerable<Tuple<RequestConfiguration, HttpTesterResults>> Run(IEnumerable<RequestConfiguration> configs)
        {
            var results = new List<Tuple<RequestConfiguration, HttpTesterResults>>(configs.Count());

            foreach (var requestConfiguration in configs)
            {
                Console.WriteLine("Running: {0}", requestConfiguration.Name);

                results.Add(Tuple.Create(requestConfiguration, RunConfig(requestConfiguration)));

                GC.Collect();
                GC.WaitForFullGCComplete();
                GC.WaitForPendingFinalizers();
            }

            return results;
        }

        private static HttpTesterResults RunConfig(RequestConfiguration config)
        {
            var tester = new HttpTester(10000, config, new AsyncHttpClient());

            return tester.Execute();
        }
    }
}
