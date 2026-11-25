using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.AppStart
{
    public class WhenAddingAuthenticationCookie
    {
        private IServiceCollection _services = null!;
        private AuthenticationBuilder _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _services = new ServiceCollection();
            _services.AddLogging();

            _sut = _services.AddAuthentication();
        }

        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase("assessor.example.com", "assessor.example.com")]
        public void Then_The_Provided_Cookie_Settings_Are_Used(
            string? cookieDomain,
            string? expectedDomain)
        {
            // Arrange
            _sut.AddAuthenticationCookie(
                "Test.Assessor.Auth",
                "/sign-out",
                cookieDomain);

            // Act
            using var provider = _services.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
                .Get(CookieAuthenticationDefaults.AuthenticationScheme);

            // Assert
            actual.Cookie.Name.Should().Be("Test.Assessor.Auth");
            actual.Cookie.Domain.Should().Be(expectedDomain);
            actual.LogoutPath.Value.Should().Be("/sign-out");
        }

        [TestCase("LOCAL", null)]
        [TestCase("TEST", "test-pas.apprenticeships.education.gov.uk")]
        [TestCase("PRD", "providers.apprenticeships.education.gov.uk")]
        public void Then_The_Existing_Overload_Uses_The_Calculated_Domain(
            string environment,
            string? expectedDomain)
        {
            // Arrange
            _sut.AddAuthenticationCookie(
                "Test.Provider.Auth",
                "/sign-out",
                environment,
                ClientName.ProviderRoatp);

            // Act
            using var provider = _services.BuildServiceProvider();

            var actual = provider
                .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
                .Get(CookieAuthenticationDefaults.AuthenticationScheme);

            // Assert
            actual.Cookie.Name.Should().Be("Test.Provider.Auth");
            actual.Cookie.Domain.Should().Be(expectedDomain);
            actual.LogoutPath.Value.Should().Be("/sign-out");
        }
    }
}