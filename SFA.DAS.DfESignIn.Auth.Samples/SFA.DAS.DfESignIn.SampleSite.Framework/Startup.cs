using Microsoft.Extensions.Configuration;
using Microsoft.Owin;
using Owin;
using SFA.DAS.Configuration.AzureTableStorage;
using System.Configuration;
using System.IO;

[assembly: OwinStartup(typeof(SFA.DAS.DfESignIn.SampleSite.Framework.Startup))]

namespace SFA.DAS.DfESignIn.SampleSite.Framework
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IConfigurationBuilder configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
            IConfiguration configuration = configurationBuilder.Build();

            var config = ConfigurationBuilderExtension.BuildConfiguration(configuration);

            var dsiConfiguration = config.GetSection("DfEOidcConfiguration").Get<DfEOidcConfiguration>();
            ConfigurationBuilderExtension.ConfigurationMarshall(dsiConfiguration);

            ConfigureAuth(app);
        }
    }
}