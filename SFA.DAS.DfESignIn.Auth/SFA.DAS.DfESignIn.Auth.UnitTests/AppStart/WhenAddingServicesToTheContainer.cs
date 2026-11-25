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
using System.Security.Claims;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.AppStart
{
    public class WhenAddingServicesToTheContainer
    {
        private IServiceCollection _sut = null!;
        private IConfigurationRoot _configuration = null!;

        [SetUp]
        public void SetUp()
        {
            _sut = new ServiceCollection();
            _configuration = GenerateConfiguration();
        }

        [TearDown]
        public void TearDown()
        {
            (_configuration as IDisposable)?.Dispose();
        }

        [TestCase(typeof(DfEOidcConfiguration))]
        [TestCase(typeof(IDfESignInService))]
        [TestCase(typeof(ITokenDataSerializer))]
        [TestCase(typeof(ITokenBuilder))]
        [TestCase(typeof(IApiHelper))]
        [TestCase(typeof(ITicketStore))]
        public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
        {
            // Arrange
            _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                ClientName.ProviderRoatp);

            // Act
            using var provider = _sut.BuildServiceProvider();

            var actual = provider.GetService(toResolve);

            // Assert
            actual.Should().NotBeNull();
        }

        [Test]
        public void Then_The_Shared_And_Client_Settings_Are_Combined()
        {
            // Arrange
            _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                "DfEOidcConfiguration_ProviderRoATP");

            // Act
            using var provider = _sut.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptions<DfEOidcConfiguration>>()
                .Value;

            // Assert
            actual.BaseUrl.Should().Be("https://test.com/");
            actual.APIServiceUrl.Should().Be("https://test-api.com/");
            actual.Scopes.Should().Be("openid email profile organisation");
            actual.LoginSlidingExpiryTimeOutInMinutes.Should().Be(30);
            actual.DfELoginSessionConnectionString.Should().Be("localhost:6379");

            actual.ClientId.Should().Be("provider-client");
            actual.Secret.Should().Be("provider-secret");
            actual.APIServiceId.Should().Be("provider-service-id");
            actual.APIServiceSecret.Should().Be("provider-api-secret");
        }

        [Test]
        public void Then_The_Explicit_Client_Section_Is_Used()
        {
            // Arrange
            _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                "DfEOidcConfiguration_Assessor");

            // Act
            using var provider = _sut.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptions<DfEOidcConfiguration>>()
                .Value;

            // Assert
            actual.ClientId.Should().Be("assessor-client");
            actual.Secret.Should().Be("assessor-secret");
            actual.APIServiceId.Should().Be("assessor-service-id");
            actual.APIServiceSecret.Should().Be("assessor-api-secret");
            actual.CookieDomain.Should().Be("assessor.example.com");
            actual.SignedOutRedirectUrl.Should().Be("/service/signed-out");

            actual.BaseUrl.Should().Be("https://test.com/");
            actual.APIServiceUrl.Should().Be("https://test-api.com/");
        }

        [TestCase(null, "openid email profile organisation")]
        [TestCase("openid email profile", "openid email profile")]
        public void Then_Assessor_Scopes_Are_Inherited_Or_Overridden(
            string? clientScopes,
            string expectedScopes)
        {
            // Arrange
            if (clientScopes != null)
            {
                _configuration["DfEOidcConfiguration_Assessor:Scopes"] =
                    clientScopes;
            }

            _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                "DfEOidcConfiguration_Assessor");

            // Act
            using var provider = _sut.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptions<DfEOidcConfiguration>>()
                .Value;

            // Assert
            actual.Scopes.Should().Be(expectedScopes);
        }

        [TestCase("")]
        [TestCase("assessor.example.com")]
        public void Then_The_Assessor_CookieDomain_Is_Bound(string cookieDomain)
        {
            // Arrange
            _configuration["DfEOidcConfiguration_Assessor:CookieDomain"] =
                cookieDomain;

            _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                "DfEOidcConfiguration_Assessor");

            // Act
            using var provider = _sut.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptions<DfEOidcConfiguration>>()
                .Value;

            // Assert
            actual.CookieDomain.Should().Be(cookieDomain);
        }

        [Test]
        public void Then_RoatpServiceAdmin_Uses_The_ServiceAdmin_Configuration()
        {
            // Arrange
            _configuration["DfEOidcConfiguration_ServiceAdmin:ClientId"] =
                "service-admin-client";

            _configuration["DfEOidcConfiguration_ServiceAdmin:Secret"] =
                "service-admin-secret";

            _configuration["DfEOidcConfiguration_ServiceAdmin:APIServiceId"] =
                "service-admin-service-id";

            _configuration["DfEOidcConfiguration_ServiceAdmin:APIServiceSecret"] =
                "service-admin-api-secret";

            _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                ClientName.RoatpServiceAdmin);

            // Act
            using var provider = _sut.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptions<DfEOidcConfiguration>>()
                .Value;

            // Assert
            actual.ClientId.Should().Be("service-admin-client");
            actual.Secret.Should().Be("service-admin-secret");
            actual.APIServiceId.Should().Be("service-admin-service-id");
            actual.APIServiceSecret.Should().Be("service-admin-api-secret");
        }

        [Test]
        public void Then_A_Missing_Shared_Section_Throws()
        {
            // Arrange
            _configuration = new ConfigurationRoot(
                new List<IConfigurationProvider>());

            // Act
            Action act = () => _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                "DfEOidcConfiguration_Assessor");

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage(
                    "Cannot find the shared configuration section 'DfEOidcConfiguration'.");
        }

        [Test]
        public void Then_A_Missing_Client_Section_Throws()
        {
            // Arrange
            // Act
            Action act = () => _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                "DfEOidcConfiguration_SupportConsole");

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage(
                    "Cannot find the client configuration section 'DfEOidcConfiguration_SupportConsole'.");
        }

        [Test]
        public void Then_A_Missing_Enum_Client_Section_Throws()
        {
            // Arrange
            // Act
            Action act = () => _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                ClientName.SupportConsole);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage(
                    "Cannot find the client configuration section 'DfEOidcConfiguration_SupportConsole'.");
        }

        [TestCase(null)]
        [TestCase("")]
        public void Then_No_Session_Connection_String_Registers_Memory_Cache(
            string? connectionString)
        {
            // Arrange
            _configuration["DfEOidcConfiguration:DfELoginSessionConnectionString"] =
                connectionString;

            _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                ClientName.ProviderRoatp);

            // Act
            using var provider = _sut.BuildServiceProvider();

            var actual = provider.GetRequiredService<IDistributedCache>();

            // Assert
            actual.Should().BeOfType<MemoryDistributedCache>();
        }

        [Test]
        public void Then_A_Session_Connection_String_Registers_Redis_Cache()
        {
            // Arrange
            _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                ClientName.ProviderRoatp);

            // Act
            using var provider = _sut.BuildServiceProvider();

            var actual = provider.GetRequiredService<IDistributedCache>();

            var options = provider
                .GetRequiredService<IOptions<RedisCacheOptions>>()
                .Value;

            // Assert
            actual.Should().BeAssignableTo<RedisCache>();
            options.Configuration.Should().Be("localhost:6379");
        }

        [Test]
        public async Task Then_Authentication_Registers_The_Expected_Schemes()
        {
            // Arrange
            _sut.AddAndConfigureDfESignInAuthentication(
                _configuration,
                "Test.DfESignIn.Auth",
                typeof(TestCustomServiceRole),
                ClientName.ProviderRoatp);

            // Act
            using var provider = _sut.BuildServiceProvider();

            var schemeProvider = provider
                .GetRequiredService<IAuthenticationSchemeProvider>();

            var schemes = await schemeProvider.GetAllSchemesAsync();

            // Assert
            schemes.Select(scheme => scheme.Name)
                .Should()
                .BeEquivalentTo(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Test]
        public void Then_Assessor_Configuration_Is_Applied_To_OpenIdConnect()
        {
            // Arrange
            _configuration["DfEOidcConfiguration:BaseUrl"] =
                "https://test.signin.example.com";

            RegisterAssessorAuthentication();

            // Act
            using var provider = _sut.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
                .Get(OpenIdConnectDefaults.AuthenticationScheme);

            // Assert
            actual.ClientId.Should().Be("assessor-client");
            actual.ClientSecret.Should().Be("assessor-secret");

            actual.MetadataAddress.Should().Be(
                "https://test.signin.example.com/.well-known/openid-configuration");

            actual.SignedOutRedirectUri.Should().Be("/service/signed-out");
            actual.CallbackPath.Value.Should().Be("/sign-in");
            actual.SignedOutCallbackPath.Value.Should().Be("/signout-callback-oidc");
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Then_Assessor_Scopes_Are_Applied_To_OpenIdConnect(
            bool overrideScopes)
        {
            // Arrange
            if (overrideScopes)
            {
                _configuration["DfEOidcConfiguration_Assessor:Scopes"] =
                    "openid email profile";
            }

            RegisterAssessorAuthentication();

            // Act
            using var provider = _sut.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
                .Get(OpenIdConnectDefaults.AuthenticationScheme);

            var expectedScopes = overrideScopes
                ? new[] { "openid", "email", "profile" }
                : new[] { "openid", "email", "profile", "organisation" };

            // Assert
            actual.Scope.Should().BeEquivalentTo(expectedScopes);
        }

        [Test]
        public void Then_Assessor_Configuration_Is_Applied_To_The_Cookie()
        {
            // Arrange
            RegisterAssessorAuthentication();

            // Act
            using var provider = _sut.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
                .Get(CookieAuthenticationDefaults.AuthenticationScheme);

            // Assert
            actual.Cookie.Name.Should().Be("Test.Assessor.Auth");
            actual.Cookie.Domain.Should().Be("assessor.example.com");
            actual.LogoutPath.Value.Should().Be("/sign-out");
            actual.ExpireTimeSpan.Should().Be(TimeSpan.FromMinutes(30));
            actual.SlidingExpiration.Should().BeTrue();
            actual.SessionStore.Should().NotBeNull();
        }

        [Test]
        public void Then_Existing_Authentication_Uses_The_Provider_Configuration()
        {
            // Arrange
            _sut.AddAndConfigureDfESignInAuthentication(
                _configuration,
                "Test.Provider.Auth",
                typeof(TestCustomServiceRole),
                ClientName.ProviderRoatp);

            // Act
            using var provider = _sut.BuildServiceProvider();

            var oidcOptions = provider
                .GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
                .Get(OpenIdConnectDefaults.AuthenticationScheme);

            var cookieOptions = provider
                .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
                .Get(CookieAuthenticationDefaults.AuthenticationScheme);

            // Assert
            oidcOptions.ClientId.Should().Be("provider-client");
            oidcOptions.ClientSecret.Should().Be("provider-secret");

            oidcOptions.Scope.Should().BeEquivalentTo(
                "openid",
                "email",
                "profile",
                "organisation");

            cookieOptions.Cookie.Name.Should().Be("Test.Provider.Auth");

            cookieOptions.Cookie.Domain.Should().Be(
                "test-pas.apprenticeships.education.gov.uk");
        }

        [Test]
        public async Task Then_Assessor_Authentication_Registers_The_Expected_Schemes()
        {
            // Arrange
            var expectedSchemeNames = new[]
            {
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
            };

            // Act
            RegisterAssessorAuthentication();

            using var provider = _sut.BuildServiceProvider();

            var schemeProvider = provider
                .GetRequiredService<IAuthenticationSchemeProvider>();

            var schemes = await schemeProvider.GetAllSchemesAsync();

            // Assert
            schemes.Select(scheme => scheme.Name)
                .Should()
                .BeEquivalentTo(expectedSchemeNames);
        }

        [Test]
        public void Then_Stub_Settings_Preserve_Each_Users_Organisations_And_Roles()
        {
            // Arrange
            var firstUserId = "11111111-1111-1111-1111-111111111111";
            var secondUserId = "22222222-2222-2222-2222-222222222222";
            var organisationAId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
            var organisationBId = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";

            var settings = new Dictionary<string, string?>
            {
                ["DfESignInStubConfiguration:DefaultUserId"] = secondUserId,

                ["DfESignInStubConfiguration:Users:0:Id"] = firstUserId,
                ["DfESignInStubConfiguration:Users:0:Email"] = "user.one@example.com",

                ["DfESignInStubConfiguration:Users:0:Organisations:0:Id"] = organisationAId,
                ["DfESignInStubConfiguration:Users:0:Organisations:0:Name"] = "Organisation A",
                ["DfESignInStubConfiguration:Users:0:Organisations:0:UkPrn"] = "10000001",

                ["DfESignInStubConfiguration:Users:0:Organisations:1:Id"] = organisationBId,
                ["DfESignInStubConfiguration:Users:0:Organisations:1:Name"] = "Organisation B",
                ["DfESignInStubConfiguration:Users:0:Organisations:1:UkPrn"] = "10000002",
                ["DfESignInStubConfiguration:Users:0:Organisations:1:Roles:0:Id"] = "33333333-3333-3333-3333-333333333333",
                ["DfESignInStubConfiguration:Users:0:Organisations:1:Roles:0:Name"] = "Administrator",
                ["DfESignInStubConfiguration:Users:0:Organisations:1:Roles:0:Code"] = "Administrator",
                ["DfESignInStubConfiguration:Users:0:Organisations:1:Roles:0:NumericId"] = "1",
                ["DfESignInStubConfiguration:Users:0:Organisations:1:Roles:0:Status:Id"] = "1",

                ["DfESignInStubConfiguration:Users:1:Id"] = secondUserId,
                ["DfESignInStubConfiguration:Users:1:Email"] = "user.two@example.com",

                ["DfESignInStubConfiguration:Users:1:Organisations:0:Id"] = organisationBId,
                ["DfESignInStubConfiguration:Users:1:Organisations:0:Name"] = "Organisation B",
                ["DfESignInStubConfiguration:Users:1:Organisations:0:UkPrn"] = "10000002",
                ["DfESignInStubConfiguration:Users:1:Organisations:0:Roles:0:Id"] = "44444444-4444-4444-4444-444444444444",
                ["DfESignInStubConfiguration:Users:1:Organisations:0:Roles:0:Name"] = "Read only",
                ["DfESignInStubConfiguration:Users:1:Organisations:0:Roles:0:Code"] = "ReadOnly",
                ["DfESignInStubConfiguration:Users:1:Organisations:0:Roles:0:NumericId"] = "2",
                ["DfESignInStubConfiguration:Users:1:Organisations:0:Roles:0:Status:Id"] = "1"
            };

            foreach (var setting in settings)
            {
                _configuration[setting.Key] = setting.Value;
            }

            // Act
            RegisterAssessorAuthentication();

            using var provider = _sut.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptions<DfESignInStubConfiguration>>()
                .Value;

            // Assert
            actual.DefaultUserId.Should().Be(secondUserId);
            actual.Users.Should().HaveCount(2);

            var firstUser = actual.Users[0];

            firstUser.Id.Should().Be(firstUserId);
            firstUser.Email.Should().Be("user.one@example.com");

            firstUser.Organisations.Select(organisation => organisation.Id)
                .Should()
                .BeEquivalentTo(new[]
                {
                    Guid.Parse(organisationAId),
                    Guid.Parse(organisationBId)
                });

            var firstUserOrganisationB = firstUser.Organisations
                .Single(organisation => organisation.Id == Guid.Parse(organisationBId));

            firstUserOrganisationB.Roles.Should().ContainSingle();
            firstUserOrganisationB.Roles[0].Code.Should().Be("Administrator");

            var secondUser = actual.Users[1];

            secondUser.Id.Should().Be(secondUserId);
            secondUser.Email.Should().Be("user.two@example.com");
            secondUser.Organisations.Should().ContainSingle();

            var secondUserOrganisationB = secondUser.Organisations[0];

            secondUserOrganisationB.Id.Should().Be(Guid.Parse(organisationBId));
            secondUserOrganisationB.Roles.Should().ContainSingle();
            secondUserOrganisationB.Roles[0].Code.Should().Be("ReadOnly");
        }

        [TestCase(null, "LOCAL", false)]
        [TestCase("false", "LOCAL", false)]
        [TestCase("true", "LOCAL", true)]
        [TestCase("true", "TEST", true)]
        [TestCase("true", "PRD", false)]
        [TestCase("true", "pRd", false)]
        public async Task Then_StubAuth_Selects_The_Expected_Authentication(
            string? stubAuth,
            string environment,
            bool expectStub)
        {
            // Arrange
            _configuration["StubAuth"] = stubAuth;
            _configuration["ResourceEnvironmentName"] = environment;

            var expectedSchemeNames = expectStub
                ? new[]
                {
            CookieAuthenticationDefaults.AuthenticationScheme
                }
                : new[]
                {
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme
                };

            var expectedChallengeScheme = expectStub
                ? CookieAuthenticationDefaults.AuthenticationScheme
                : OpenIdConnectDefaults.AuthenticationScheme;

            // Act
            RegisterAssessorAuthentication();

            using var provider = _sut.BuildServiceProvider();

            var schemeProvider = provider
                .GetRequiredService<IAuthenticationSchemeProvider>();

            var schemes = await schemeProvider.GetAllSchemesAsync();

            var authenticationOptions = provider
                .GetRequiredService<IOptions<AuthenticationOptions>>()
                .Value;

            // Assert
            schemes.Select(scheme => scheme.Name)
                .Should()
                .BeEquivalentTo(expectedSchemeNames);

            authenticationOptions.DefaultChallengeScheme
                .Should()
                .Be(expectedChallengeScheme);
        }

        [Test]
        public void Then_Stub_Authentication_Configures_The_Login_Cookie()
        {
            // Arrange
            _configuration["StubAuth"] = "true";
            _configuration["ResourceEnvironmentName"] = "LOCAL";

            // Act
            RegisterAssessorAuthentication();

            using var provider = _sut.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
                .Get(CookieAuthenticationDefaults.AuthenticationScheme);

            // Assert
            actual.Cookie.Name.Should().Be("Test.Assessor.Auth.stub");
            actual.Cookie.Domain.Should().Be("assessor.example.com");
            actual.LoginPath.Value.Should().Be("/stub/sign-in-stub");
            actual.LogoutPath.Value.Should().Be("/sign-out");
            actual.ExpireTimeSpan.Should().Be(TimeSpan.FromMinutes(30));
            actual.SlidingExpiration.Should().BeTrue();

            actual.SessionStore.Should().BeSameAs(
                provider.GetRequiredService<ITicketStore>());
        }

        private void RegisterAssessorAuthentication()
        {
            _configuration["DfEOidcConfiguration:DfELoginSessionConnectionString"] =
                string.Empty;

            _sut.AddAndConfigureDfESignInAuthentication(
                _configuration,
                "Test.Assessor.Auth",
                typeof(TestCustomServiceRole),
                "DfEOidcConfiguration_Assessor",
                "/sign-out");
        }

        private static IConfigurationRoot GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource
            {
                InitialData = new List<KeyValuePair<string, string?>>
                {
                    new("DfEOidcConfiguration:BaseUrl", "https://test.com/"),
                    new("DfEOidcConfiguration:APIServiceUrl", "https://test-api.com/"),
                    new("DfEOidcConfiguration:ResponseType", "code"),
                    new("DfEOidcConfiguration:KeyVaultIdentifier", "test-key-vault"),
                    new("DfEOidcConfiguration:Scopes", "openid email profile organisation"),
                    new("DfEOidcConfiguration:APIServiceAudience", "signin.education.gov.uk"),
                    new("DfEOidcConfiguration:DfELoginSessionConnectionString", "localhost:6379"),
                    new("DfEOidcConfiguration:LoginSlidingExpiryTimeOutInMinutes", "30"),

                    new("DfEOidcConfiguration_ProviderRoATP:ClientId", "provider-client"),
                    new("DfEOidcConfiguration_ProviderRoATP:Secret", "provider-secret"),
                    new("DfEOidcConfiguration_ProviderRoATP:APIServiceId", "provider-service-id"),
                    new("DfEOidcConfiguration_ProviderRoATP:APIServiceSecret", "provider-api-secret"),

                    new("DfEOidcConfiguration_Assessor:ClientId", "assessor-client"),
                    new("DfEOidcConfiguration_Assessor:Secret", "assessor-secret"),
                    new("DfEOidcConfiguration_Assessor:APIServiceId", "assessor-service-id"),
                    new("DfEOidcConfiguration_Assessor:APIServiceSecret", "assessor-api-secret"),
                    new("DfEOidcConfiguration_Assessor:CookieDomain", "assessor.example.com"),
                    new("DfEOidcConfiguration_Assessor:SignedOutRedirectUrl", "/service/signed-out"),

                    new("ResourceEnvironmentName", "test")
                }
            };

            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(
                new List<IConfigurationProvider> { provider });
        }

        [Test]
        public void Then_The_Legacy_Stub_Registration_Allows_A_Missing_Client_Section()
        {
            // Arrange
            const string missingClientSection =
                "DfEOidcConfiguration_SupportConsole";

            // Act
            _sut.AddServiceRegistration(
                _configuration,
                typeof(TestCustomServiceRole),
                ClientName.SupportConsole,
                useStub: true);

            using var provider = _sut.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptions<DfEOidcConfiguration>>()
                .Value;

            // Assert
            _configuration.GetSection(missingClientSection)
                .GetChildren().Should().BeEmpty();

            actual.BaseUrl.Should().Be("https://test.com/");
            actual.APIServiceUrl.Should().Be("https://test-api.com/");
            actual.ClientId.Should().BeNull();
        }

        [Test]
        public async Task Then_NoAuthEmail_Allows_The_Legacy_Stub_Without_Client_Configuration()
        {
            // Arrange
            _configuration["NoAuthEmail"] = "stub@example.com";

            _configuration["DfEOidcConfiguration:DfELoginSessionConnectionString"] =
                string.Empty;

            // Act
            _sut.AddAndConfigureDfESignInAuthentication(
                _configuration,
                "Test.Legacy.Auth",
                typeof(TestCustomServiceRole),
                ClientName.SupportConsole);

            using var provider = _sut.BuildServiceProvider();

            var schemeProvider = provider
                .GetRequiredService<IAuthenticationSchemeProvider>();

            var actual = await schemeProvider
                .GetDefaultAuthenticateSchemeAsync();

            // Assert
            actual.Should().NotBeNull();
            actual!.Name.Should().Be("Test.Legacy.Auth.stub");
            actual.HandlerType.Should().Be(typeof(ProviderStubAuthHandler));
        }

        [TestCase(null)]
        [TestCase("")]
        public void Then_Real_Authentication_Requires_The_Client_Section(
            string? noAuthEmail)
        {
            // Arrange
            _configuration["NoAuthEmail"] = noAuthEmail;

            // Act
            Action act = () => _sut.AddAndConfigureDfESignInAuthentication(
                _configuration,
                "Test.Legacy.Auth",
                typeof(TestCustomServiceRole),
                ClientName.SupportConsole);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage(
                    "Cannot find the client configuration section 'DfEOidcConfiguration_SupportConsole'.");
        }

        public class TestCustomClaims : ICustomClaims
        {
            public IEnumerable<Claim> GetClaims(TokenValidatedContext tokenValidatedContext)
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<Claim>> GetClaimsAsync(TokenValidatedContext tokenValidatedContext)
            {
                throw new NotImplementedException();
            }
        }

        public class TestCustomServiceRole : ICustomServiceRole
        {
            public string RoleClaimType => ClaimTypes.Role;

            public CustomServiceRoleValueType RoleValueType =>
                CustomServiceRoleValueType.Name;
        }
    }
}