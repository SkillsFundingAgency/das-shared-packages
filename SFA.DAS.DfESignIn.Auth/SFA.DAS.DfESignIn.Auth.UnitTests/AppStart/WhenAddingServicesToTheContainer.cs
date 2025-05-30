using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.AppStart;

public class WhenAddingServicesToTheContainer
{
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

        type.Should().NotBeNull();
    }

    [Test]
    public void Then_DfELoginSessionConnectionString_IsNull_AddDistributedMemoryCache_RegistersDistributedMemoryCache()
    {
        // Arrange
        var configuration = GenerateConfiguration();
        configuration["DfEOidcConfiguration:DfELoginSessionConnectionString"] = string.Empty;

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomServiceRole), ClientName.ProviderRoatp);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var distributedCache = serviceProvider.GetService<IDistributedCache>();
        distributedCache.Should().NotBeNull();
        distributedCache.Should().BeOfType<MemoryDistributedCache>();
    }

    [Test]
    public void Then_DfELoginSessionConnectionString_IsNotNull_AddStackExchangeRedisCache_RegistersDistributedMemoryCache()
    {
        // Arrange
        var configuration = GenerateConfiguration();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomServiceRole), ClientName.ProviderRoatp);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var distributedCache = serviceProvider.GetService<IDistributedCache>();
        distributedCache.Should().NotBeNull();
        distributedCache.Should().BeAssignableTo<RedisCache>();

        // We can't test the Redis connection details directly in .NET 8.0 as the implementation details have changed
        // Instead, we'll verify that the service is registered correctly
        var redisOptions = serviceProvider.GetService<IOptions<RedisCacheOptions>>();
        redisOptions.Should().NotBeNull();
        redisOptions!.Value.Configuration.Should().Be("https://test.com/");
    }

    [Test]
    public async Task Then_ConfigureDfESignInAuthentication_Should_Have_Expected_AuthenticationCookie()
    {
        // Arrange
        var configuration = GenerateConfiguration();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddAndConfigureDfESignInAuthentication(configuration, $"{typeof(AddServiceRegistrationExtension).Assembly.GetName().Name}.Auth", typeof(TestCustomServiceRole), ClientName.ProviderRoatp);
        var expectedAuthSchemeNames = new[] { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme };

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var authenticationSchemeProvider = serviceProvider.GetService<IAuthenticationSchemeProvider>();
        authenticationSchemeProvider.Should().NotBeNull();
        
        var authenticationSchemes = await authenticationSchemeProvider!.GetAllSchemesAsync();
        var authSchemeList = authenticationSchemes.ToList();
        var actualAuthSchemeNames = authSchemeList.Select(args => args.Name);
        
        authSchemeList.Should().NotBeNull();
        actualAuthSchemeNames.Should().BeSubsetOf(expectedAuthSchemeNames);
    }

    private static void SetupServiceCollection(IServiceCollection serviceCollection)
    {
        var configuration = GenerateConfiguration();
        serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomServiceRole), ClientName.ProviderRoatp);
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string?>>
            {
                new("DfEOidcConfiguration:BaseUrl", "https://test.com/"),
                new("DfEOidcConfiguration:ClientId", "1234567"),
                new("DfEOidcConfiguration:APIServiceSecret", "1234567"),
                new("DfEOidcConfiguration:KeyVaultIdentifier", "https://test.com/"),
                new("ProviderSharedUIConfiguration:DashboardUrl", "https://test.com/"),
                new("DfEOidcConfiguration:DfELoginSessionConnectionString", "https://test.com/"),
                new("DfEOidcConfiguration:LoginSlidingExpiryTimeOutInMinutes", "30"),
                new("ResourceEnvironmentName", "test")
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }

    public class TestCustomClaims : ICustomClaims
    {
        public IEnumerable<Claim?> GetClaims(TokenValidatedContext tokenValidatedContext) =>
            throw new NotImplementedException();
    }

    public class TestCustomServiceRole : ICustomServiceRole
    {
        public string RoleClaimType => throw new NotImplementedException();
        public CustomServiceRoleValueType RoleValueType => CustomServiceRoleValueType.Name;
    }
}