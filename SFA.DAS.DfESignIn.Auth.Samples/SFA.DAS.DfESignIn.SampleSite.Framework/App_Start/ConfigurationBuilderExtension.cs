using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace SFA.DAS.DfESignIn.SampleSite.Framework
{
    public class ConfigurationBuilderExtension
    {
        public static IConfigurationRoot BuildConfiguration(IConfiguration configuration)
        {
            var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddConfiguration(configuration);

            config.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = new string[] { "SFA.DAS.Provider.DfeSignIn" };
                options.StorageConnectionString = "UseDevelopmentStorage=true;";
                options.EnvironmentName = "LOCAL";
                options.PreFixConfigurationKeys = false;
            });

            return config.Build();
        }

        public static IConfigurationRoot GetConfigurationRoot()
        { 
            IConfigurationBuilder configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
            IConfiguration configuration = configurationBuilder.Build();
            return ConfigurationBuilderExtension.BuildConfiguration(configuration);
        }

        public static void ConfigurationMarshall(DfEOidcConfiguration configuration)
        {
            ConfigurationManager.AppSettings["BaseUrl"] = configuration.BaseUrl;
            ConfigurationManager.AppSettings["ClientId"] = configuration.ClientId;
            ConfigurationManager.AppSettings["Secret"] = configuration.Secret;
            ConfigurationManager.AppSettings["APIServiceUrl"] = configuration.APIServiceUrl;
            ConfigurationManager.AppSettings["APIServiceId"] = configuration.APIServiceId;
            ConfigurationManager.AppSettings["APIServiceSecret"] = configuration.APIServiceSecret;
        }
    }
}