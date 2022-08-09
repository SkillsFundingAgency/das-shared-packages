using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Interfaces;

namespace SFA.DAS.GovUK.Auth.UnitTests.AppStart;

public class WhenAddingServicesToTheContainer
{
    
    [TestCase(typeof(IOidcService))]
    [TestCase(typeof(IAzureIdentityService))]
    [TestCase(typeof(IJwtSecurityTokenService))]
    public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
            
        Assert.IsNotNull(type);
    }

    private void SetupServiceCollection(ServiceCollection serviceCollection)
    {   
        var configuration = GenerateConfiguration();
        serviceCollection.AddServiceRegistration(configuration);
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("GovUkOidcConfiguration:BaseUrl", "https://test.com/")
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}