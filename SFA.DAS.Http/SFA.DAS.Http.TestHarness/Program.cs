using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Http.TestHarness
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var host = CreateWebHostBuilder(args).Build())
            {
                host.Start();
                
                var httpClient = new HttpClientBuilder()
                    .WithDefaultHeaders()
                    .WithLogging(host.Services.GetService<ILoggerFactory>())
                    .Build();
                
                httpClient.BaseAddress = new Uri("http://localhost:5000/api/");
                
                var restClient = new RestHttpClient(httpClient);
                var result = restClient.Get("test").GetAwaiter().GetResult();
                
                Console.WriteLine($"API response content: '{result}'");
                Console.WriteLine("Press any key to quit...");
                Console.Read();
                
                host.StopAsync().Wait();
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            new WebHostBuilder()
                .ConfigureLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace))
                .UseKestrel()
                .UseStartup<Startup>();
    }
}