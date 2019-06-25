using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SFA.DAS.EmployerUrlHelper.Sample.Startup;

namespace SFA.DAS.EmployerUrlHelper.Sample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration()
                .UseStartup<AspNetStartup>();
    }
}