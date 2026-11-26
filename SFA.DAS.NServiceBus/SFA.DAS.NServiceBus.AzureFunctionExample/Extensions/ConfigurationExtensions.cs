using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.NServiceBus.AzureFunctionExample.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfiguration BuildDasConfiguration(this IConfigurationBuilder configBuilder, IConfiguration configuration)
        {
            configBuilder
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            configBuilder.AddJsonFile("local.settings.json", optional: true);

            return configBuilder.Build();
        }
    }
}
