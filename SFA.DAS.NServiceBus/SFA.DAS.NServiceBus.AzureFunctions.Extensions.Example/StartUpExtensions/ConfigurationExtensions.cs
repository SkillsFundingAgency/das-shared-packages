using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.NServiceBus.AzureFunctions.Extensions.Example.StartUpExtensions
{
    public static class ConfigurationExtensions
    {
        public static IConfiguration BuildDasConfiguration(this IConfigurationBuilder configBuilder)
        {
            configBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true);

            return configBuilder.Build();
        }
    }
}
