using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.DfESignIn.Auth.Configuration;
using System;
using System.IO;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    public static class ConfigurationBuilderExtension
    {
        public static IConfigurationRoot BuildConfiguration(this IConfiguration configuration)
        {
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            if (!configuration.IsLocalAcceptanceTestsOrDev())
            {
                config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(new char[',']);
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["Environment"];
                    options.PreFixConfigurationKeys = false;
                }
                );
            }
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
            return configuration["Environment"].Equals("LOCAL_ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsDev(this IConfiguration configuration)
        {
            return configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }
        public static bool IsLocal(this IConfiguration configuration)
        {
            return configuration["Environment"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}