using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Configuration.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new HostBuilder()
                .ConfigureAppConfiguration((c, b) => b
                .AddAzureTableStorage(o =>
                {
                    o.EnvironmentNameEnvironmentVariableName = "";
                    o.ConfigurationNameIncludesVersionNumber = false;
                    o.ConfigurationKeys = new[]
                    {
                        "DoesntExist"
                    };
                }))
                .Build())
            {
            }
        }
    }
}