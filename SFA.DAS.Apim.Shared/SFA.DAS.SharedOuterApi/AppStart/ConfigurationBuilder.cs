using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.SharedOuterApi.AppStart
{
    public static class SharedConfigurationBuilderExtension
    {
        public static IConfigurationRoot BuildSharedConfiguration(this IConfiguration configuration)
        {
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
            config.AddJsonFile("appsettings.json", true);
            if (!configuration.IsLocalAcceptanceTests())
            {
                config.AddJsonFile("appsettings.Development.json", true);
            }
#endif
            if (!configuration.IsLocalAcceptanceTestsOrDev())
            {
                config.AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = configuration["ConfigNames"]!.Split(",");
                        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"]!;
                        options.EnvironmentName = configuration["Environment"]!;
                        options.PreFixConfigurationKeys = false;
                    }
                );
            }

            config.AddEnvironmentVariables();

            return config.Build();
        }

        public static bool IsLocalOrDev(this IConfiguration configuration)
        {
            return configuration.IsLocal() || configuration.IsDev() || configuration.IsLocalAcceptanceTests();
        }

        public static bool IsLocalAcceptanceTestsOrDev(this IConfiguration configuration)
        {
            return configuration.IsLocalAcceptanceTests() || configuration.IsDev();
        }

        public static bool IsLocalAcceptanceTests(this IConfiguration configuration)
        {
            return configuration["Environment"]!.Equals("LOCAL_ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsDev(this IConfiguration configuration)
        {
            return configuration["Environment"]!.Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }
        public static bool IsLocal(this IConfiguration configuration)
        {
            return configuration["Environment"]!.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}