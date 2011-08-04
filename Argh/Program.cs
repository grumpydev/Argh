namespace Argh
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Argh.DSL;

    using HttpClient;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nAsync Request Generator for HTTP (ARGH)\n");

            var config = new Config(args);

            if (String.IsNullOrWhiteSpace(config.InputFile) || !File.Exists(config.InputFile))
            {
                ShowUsage();
                return;
            }

            var settings = Settings.Load(config.InputFile);

            var results = Run(settings).ToArray();

            Console.WriteLine("\nResults:\n");
            foreach (var result in results)
            {
                Console.WriteLine(result.Item1.Name);

                if (result.Item2.Errors.Any())
                {
                    Console.WriteLine();
                    Console.WriteLine("Errors:");

                    foreach (var error in result.Item2.Errors)
                    {
                        Console.WriteLine(error);
                        Console.WriteLine("-----------");
                    }
                }

                Console.WriteLine("Completed {0} iterations, {1} errors, {2} peak simultaneous, {3:0.00} seconds, {4:0.00} req/sec", result.Item2.TotalExecuted, result.Item2.Errored, result.Item2.PeakSimultaneous, result.Item2.ExecutionTime.TotalSeconds, result.Item2.TotalExecuted / result.Item2.ExecutionTime.TotalSeconds);

                Console.WriteLine();
            }

            if (!String.IsNullOrWhiteSpace(config.OutputFile))
            {
                WriteOutputToFile(results, config.OutputFile);
            }
        }

        private static void WriteOutputToFile(IEnumerable<Tuple<RequestConfiguration, HttpTesterResults>> results, string outputFile)
        {
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            using (var output = File.CreateText(outputFile))
            {
                output.WriteLine(@"TestName,Iterations,Errors,PeakSimul,SecondsToExecute,RequestPerSecond");

                foreach (var result in results)
                {
                    output.WriteLine("'{0}',{1},{2},{3},{4:0.00},{5:0.00}", result.Item1.Name.Replace("'", "''"), result.Item2.TotalExecuted, result.Item2.Errored, result.Item2.PeakSimultaneous, result.Item2.ExecutionTime.TotalSeconds, result.Item2.TotalExecuted / result.Item2.ExecutionTime.TotalSeconds);

                    if (result.Item2.Errors.Any())
                    {
                        output.WriteLine();
                        output.WriteLine("Errors:");

                        foreach (var error in result.Item2.Errors)
                        {
                            output.WriteLine(error);
                            output.WriteLine("-----------");
                        }
                    }
                }

                output.Close();
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("No configuration file specified.\n");
            Console.WriteLine("Usage:");
            Console.WriteLine("\tArgh [-o:outputfilename.csv] <config filename>\n\n");
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
                Thread.Sleep(1000);
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
