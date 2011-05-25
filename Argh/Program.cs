namespace Argh
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Argh.DSL;

    using HttpClient;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nAsync Request Generator for HTTP (ARGH)\n");

            if (args.Length != 1)
            {
                ShowUsage();
                return;
            }

            var settings = Settings.Load(args[0]);

            var results = Run(settings);

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

        private static void ShowUsage()
        {
            Console.WriteLine("No configuration file specified.\n");
            Console.WriteLine("Usage:");
            Console.WriteLine("\tArgh <config filename>\n\n");
            Console.WriteLine("Sample config file: \n");

            Console.WriteLine("Iterations = 4000");
            Console.WriteLine();
            Console.WriteLine("RequestConfiguration \"Get Request\":");
            Console.WriteLine("\tmethod @Get");
            Console.WriteLine("\turl \"http://localhost:40965/test\"");
            Console.WriteLine("\tno_cache true");
            Console.WriteLine("\theaders { ");
            Console.WriteLine("\t	Testing:\"my value\",");
            Console.WriteLine("\t	AnotherHeader:\"another value\"");
            Console.WriteLine("\t}");
            Console.WriteLine();
            Console.WriteLine("RequestConfiguration \"Manual Form Post\":");
            Console.WriteLine("\tmethod @Post");
            Console.WriteLine("\turl \"http://localhost:40965/spark\"");
            Console.WriteLine("\tno_cache true");
            Console.WriteLine("\tcontent_type \"application/x-www-form-urlencoded\"");
            Console.WriteLine("\tbody \"testing=qwdqw\"");
            return;
        }

        private static IEnumerable<Tuple<RequestConfiguration, HttpTesterResults>> Run(ArghSettings settings)
        {
            var results = new List<Tuple<RequestConfiguration, HttpTesterResults>>(settings.RequestConfigurations.Count());

            foreach (var requestConfiguration in settings.RequestConfigurations)
            {
                Console.WriteLine("Running: {0}", requestConfiguration.Name);

                results.Add(Tuple.Create(requestConfiguration, RunConfig(requestConfiguration, settings.Iterations)));

                GC.Collect();
                GC.WaitForFullGCComplete();
                GC.WaitForPendingFinalizers();
            }

            return results;
        }

        private static HttpTesterResults RunConfig(RequestConfiguration config, int iterations)
        {
            var tester = new HttpTester(iterations, config, new AsyncHttpClient());

            return tester.Execute();
        }
    }
}
