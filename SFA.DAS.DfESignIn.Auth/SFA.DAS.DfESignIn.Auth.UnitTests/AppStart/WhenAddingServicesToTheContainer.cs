using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.AspNetCore.Authentication;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.AppStart;


public class WhenAddingServicesToTheContainer
{
    private const string ClientName = "someProvider";

    [TestCase(typeof(DfEOidcConfiguration))]
    [TestCase(typeof(IDfESignInService))]
    [TestCase(typeof(ITokenDataSerializer))]
    [TestCase(typeof(ITokenBuilder))]
    [TestCase(typeof(IApiHelper))]
    [TestCase(typeof(ITicketStore))]
    public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);

        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);

        Assert.That(type, Is.Not.Null);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    public void Then_The_ClientName_Given_NullOrEmpty_Throw_Exception(string clientName)
    {
        // arrange
        var configuration = GenerateConfiguration();
        var serviceCollection = new ServiceCollection();

        // sut
        Assert.Throws<ArgumentNullException>(() => 
            serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomServiceRole), clientName));
    }

    [Test]
    public void Then_The_Configuration_Given_NullOrEmpty_Throw_Exception()
    {
        // arrange
        var configuration = new ConfigurationRoot(new List<IConfigurationProvider>
        { 
            new MemoryConfigurationProvider(new MemoryConfigurationSource
            {
                InitialData = new List<KeyValuePair<string, string>>()

            })
        });
        var serviceCollection = new ServiceCollection();

        // sut
        Assert.Throws<ArgumentException>(() =>
            serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomServiceRole), It.IsAny<string>()));
    }

    [Test]
    public void Then_DfELoginSessionConnectionString_IsNull_AddDistributedMemoryCache_RegistersDistributedMemoryCache()
    {
        // Arrange
        var configuration = GenerateConfiguration();
        configuration["DfEOidcConfiguration:DfELoginSessionConnectionString"] = string.Empty;

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomServiceRole), ClientName);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var distributedCache = serviceProvider.GetService<IDistributedCache>();
        Assert.That(distributedCache, Is.Not.Null);
        Assert.That(distributedCache, Is.InstanceOf<MemoryDistributedCache>());
    }

    [Test]
    public void Then_DfELoginSessionConnectionString_IsNotNull_AddStackExchangeRedisCache_RegistersDistributedMemoryCache()
    {
        // Arrange
        var configuration = GenerateConfiguration();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomServiceRole), ClientName);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var distributedCache = serviceProvider.GetService<IDistributedCache>();
        Assert.That(distributedCache, Is.Not.Null);
        Assert.That(distributedCache, Is.InstanceOf<RedisCache>());
    }

    [Test]
    public async Task Then_ConfigureDfESignInAuthentication_Should_Have_Expected_AuthenticationCookie()
    {
        // Arrange
        var configuration = GenerateConfiguration();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddAndConfigureDfESignInAuthentication(configuration, $"{typeof(AddServiceRegistrationExtension).Assembly.GetName().Name}.Auth", typeof(TestCustomServiceRole), ClientName);
        var expectedAuthSchemeNames = new[] { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme };

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var authenticationSchemeProvider = serviceProvider.GetService<IAuthenticationSchemeProvider>();
        Assert.That(authenticationSchemeProvider, Is.Not.Null);
        
        var authenticationSchemes = await authenticationSchemeProvider?.GetAllSchemesAsync()!;
        var authSchemeList = authenticationSchemes?.ToList();
        var actualAuthSchemeNames = authSchemeList?.Select(args => args.Name);
        Assert.Multiple(() =>
        {
            Assert.That(authSchemeList, Is.Not.Null);
            Assert.That(actualAuthSchemeNames, Is.SupersetOf(expectedAuthSchemeNames));
        });
    }

    private static void SetupServiceCollection(IServiceCollection serviceCollection)
    {
        var configuration = GenerateConfiguration();
        serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomServiceRole), ClientName);
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("DfEOidcConfiguration:BaseUrl", "https://test.com/"),
                new("DfEOidcConfiguration:ClientId", "1234567"),
                new("DfEOidcConfiguration:APIServiceSecret", "1234567"),
                new("DfEOidcConfiguration:KeyVaultIdentifier", "https://test.com/"),
                new("ProviderSharedUIConfiguration:DashboardUrl", "https://test.com/"),
                new("DfEOidcConfiguration:DfELoginSessionConnectionString", "https://test.com/"),
                new("DfEOidcConfiguration:LoginSlidingExpiryTimeOutInMinutes", "30"),
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }

    public class TestCustomClaims : ICustomClaims
    {
        public IEnumerable<Claim?> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            throw new NotImplementedException();
        }
    }

    public class TestCustomServiceRole : ICustomServiceRole
    {
        public string RoleClaimType => throw new NotImplementedException();
        public CustomServiceRoleValueType RoleValueType => CustomServiceRoleValueType.Name;
    }
}