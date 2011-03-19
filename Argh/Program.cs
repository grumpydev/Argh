namespace Argh
{
    using System;

    using HttpClient;

    class Program
    {
        static void Main(string[] args)
        {
            var config = new RequestConfiguration()
                {
                    Method = HttpMethod.Post,
                    ContentType = "application/x-www-form-urlencoded",
                    Url = "http://localhost:8080/",
                    NoCache = true,
                    RequestBody = "testing=qwdqw",
                };

            Console.WriteLine("Running..");

            var tester = new HttpTester(10000, config, new AsyncHttpClient());

            var results = tester.Execute();

            Console.WriteLine("Completed {0} iterations, {1} errors, {2} peak simultaneous, {3:0.00} seconds, {4:0.00} req/sec", results.TotalExecuted, results.Errored, results.PeakSimultaneous, results.ExecutionTime.TotalSeconds, results.TotalExecuted / results.ExecutionTime.TotalSeconds);

            Console.ReadLine();
        }
    }
}
