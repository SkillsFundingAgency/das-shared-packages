using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using System.Configuration;

namespace SFA.DAS.DfESignIn.SampleSite.Classic
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

        public static void ConfigurationMarshall(DfEOidcConfiguration configuration)
        {
            ConfigurationManager.AppSettings["BaseUrl"] = configuration.BaseUrl;
            ConfigurationManager.AppSettings["ClientId"] = configuration.ClientId;
            ConfigurationManager.AppSettings["KeyVaultIdentifier"] = configuration.KeyVaultIdentifier;
            ConfigurationManager.AppSettings["Secret"] = configuration.Secret;
            ConfigurationManager.AppSettings["ResponseType"] = configuration.ResponseType;
            ConfigurationManager.AppSettings["Scopes"] = configuration.Scopes;
            ConfigurationManager.AppSettings["APIServiceUrl"] = configuration.APIServiceUrl;
            ConfigurationManager.AppSettings["APIServiceId"] = configuration.APIServiceId;
            ConfigurationManager.AppSettings["APIServiceAudience"] = configuration.APIServiceAudience;
            ConfigurationManager.AppSettings["APIServiceSecret"] = configuration.APIServiceSecret;
        }
    }
}