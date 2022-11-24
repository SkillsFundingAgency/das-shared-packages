using System.Net.Http;
using Microsoft.Extensions.Configuration;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Services;
using StructureMap;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    public class GovUkStructureMap : Registry
    {
        public GovUkStructureMap(string configKey)
        {
            AddConfiguration<GovUkOidcConfiguration>(configKey);
            For<IOidcService>().Use<OidcService>().Ctor<HttpClient>().Is(new HttpClient());
            For<IAzureIdentityService>().Use<AzureIdentityService>();
            For<IJwtSecurityTokenService>().Use<JwtSecurityTokenService>();
        }
        private void AddConfiguration<T>(string key) where T : class
        {
            For<T>().Use(c => GetConfiguration<T>(c, key)).Singleton();
        }
        private T GetConfiguration<T>(IContext context, string name)
        {
            var configuration = context.GetInstance<IConfiguration>();
            var section = configuration.GetSection(name);
            var value = section.Get<T>();

            return value;
        }
    }
}