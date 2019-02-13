using System;
using Microsoft.AspNetCore.Hosting;

namespace SFA.DAS.Http.TestHarness
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();
 
            host.Start();
            
            Console.WriteLine("Creating Http client");
            var httpClient = new HttpClientBuilder().WithDefaultHeaders().Build();
            httpClient.BaseAddress = new Uri("http://localhost:5000/api/");

            Console.WriteLine("Creating REST client");
            var restClient = new RestHttpClient(httpClient);

            Console.WriteLine("Calling test server api to get test value...");
            var result = restClient.Get("test").Result;
            
            Console.WriteLine($"Api Test Call Result: {result}");

            Console.WriteLine("Press any key to quit...");
            Console.Read();

            host.StopAsync().Wait();
        }
    }
}