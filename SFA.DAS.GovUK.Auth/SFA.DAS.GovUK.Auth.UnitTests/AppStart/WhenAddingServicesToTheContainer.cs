using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Employer;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.UnitTests.AppStart
{

    public class WhenAddingServicesToTheContainer
    {
        private VerifiedIdentityFailureHandler _sut = null!;

        [TestCase(typeof(IGovUkAuthenticationService))]
        [TestCase(typeof(IAzureIdentityService))]
        [TestCase(typeof(IJwtSecurityTokenService))]
        [TestCase(typeof(ICustomClaims))]
        [TestCase(typeof(IStubAuthenticationService))]
        [TestCase(typeof(IGovAuthEmployerAccountService))]
        public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
        {
            var serviceCollection = new ServiceCollection();
            SetupServiceCollection(serviceCollection);

            var provider = serviceCollection.BuildServiceProvider();

            var type = provider.GetService(toResolve);

            Assert.That(type, Is.Not.Null);
        }

        [Test]
        public void Then_Resolves_Authorization_Handlers()
        {
            var serviceCollection = new ServiceCollection();
            SetupServiceCollection(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            var type = provider.GetServices(typeof(IAuthorizationHandler)).ToList();

            Assert.That(type, Is.Not.Null);
            type.Count.Should().Be(2);
            type.Should().Contain(c => c!.GetType() == typeof(AccountActiveAuthorizationHandler));
            type.Should().Contain(c => c!.GetType() == typeof(VerifiedIdentityAuthorizationHandler));
        }

        [Test]
        public async Task Then_The_Configured_Verify_Identity_Information_Url_Is_Passed_To_The_Handler()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            var authRedirects = new AuthRedirects
            {
                VerifyIdentityInformationUrl = "/explain-verify"
            };

            SetupServiceCollection(serviceCollection, authRedirects);

            using var provider = serviceCollection.BuildServiceProvider();

            _sut = provider
                .GetServices<IAuthorizationFailureHandler>()
                .OfType<VerifiedIdentityFailureHandler>()
                .Single();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/secure/resource";
            httpContext.Request.QueryString = new QueryString("?x=1");

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new VerifiedIdentityRequirement())
                .Build();

            var authorizationHandler = Mock.Of<IAuthorizationHandler>();

            var failure = AuthorizationFailure.Failed(new[]
            {
                new AuthorizationFailureReason(
                    authorizationHandler,
                    AuthorizationFailureMessages.NotVerified)
            });

            var result = PolicyAuthorizationResult.Forbid(failure);

            // Act
            var handled = await _sut.HandleFailureAsync(
                httpContext,
                policy,
                result);

            // Assert
            handled.Should().BeTrue();

            httpContext.Response.StatusCode.Should()
                .Be(StatusCodes.Status302Found);

            httpContext.Response.Headers.Location.ToString().Should().Be(
                "/explain-verify?returnUrl=%2Fsecure%2Fresource%3Fx%3D1");
        }

        private static void SetupServiceCollection(
            IServiceCollection serviceCollection,
            AuthRedirects? authRedirects = null)
        {
            var configuration = GenerateConfiguration();

            serviceCollection.AddSingleton<IConfiguration>(configuration);

            serviceCollection.AddServiceRegistration(
                configuration,
                authRedirects ?? new AuthRedirects(),
                typeof(TestCustomClaims),
                typeof(GovAuthEmployerAccountService));
        }

        private static IConfigurationRoot GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource
            {
                InitialData = new List<KeyValuePair<string, string>>
                {
                    new("GovUkOidcConfiguration:BaseUrl", "https://test.com/"),
                    new("GovUkOidcConfiguration:ClientId", "1234567"),
                    new("GovUkOidcConfiguration:KeyVaultIdentifier", "https://test.com/"),
                    new("ResourceEnvironmentName", "AT")
                }!
            };

            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }

        public class TestCustomClaims : ICustomClaims
        {
            public Task<IEnumerable<Claim?>> GetClaims(TokenValidatedContext tokenValidatedContext)
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<Claim>> GetClaims(ClaimsPrincipal principal)
            {
                throw new NotImplementedException();
            }
        }
        public class GovAuthEmployerAccountService : IGovAuthEmployerAccountService
        {
            public Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
            {
                throw new NotImplementedException();
            }
        }
    }
}