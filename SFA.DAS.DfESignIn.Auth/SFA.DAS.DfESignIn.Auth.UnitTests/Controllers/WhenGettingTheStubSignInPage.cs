using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Models;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Controllers;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Models.Stub;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Controllers
{
    public class WhenGettingTheStubSignInPage
    {
        private IConfigurationRoot _configuration = null!;
        private DfESignInStubConfiguration _stubConfiguration = null!;
        private Mock<IDfESignInStubService> _mockDfeSignInStubService = null!;
        private DfESignInStubController _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["StubAuth"] = "true",
                    ["ResourceEnvironmentName"] = "LOCAL"
                })
                .Build();

            _stubConfiguration = new DfESignInStubConfiguration
            {
                DefaultUserId = "user-two",
                Users = new List<DfESignInStubUser>
                {
                    new DfESignInStubUser
                    {
                        Id = "user-one",
                        Email = "user.one@example.com"
                    },
                    new DfESignInStubUser
                    {
                        Id = "user-two",
                        Email = "user.two@example.com"
                    }
                }
            };

            _mockDfeSignInStubService = new Mock<IDfESignInStubService>();

            _sut = new DfESignInStubController(
                _configuration,
                Options.Create(_stubConfiguration),
                _mockDfeSignInStubService.Object,
                Options.Create(new DfEOidcConfiguration
                {
                    ClientId = "assessor-client"
                }));
        }

        [TearDown]
        public void TearDown()
        {
            _sut.Dispose();
            (_configuration as IDisposable)?.Dispose();
        }

        [Test]
        public void Then_The_Configured_Users_And_Default_Selection_Are_Shown()
        {
            // Arrange
            var returnUrl = "/requested-page";

            // Act
            var result = _sut.SignInStub(returnUrl);

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Which;

            view.ViewName.Should().Be("SignInStub");

            var model = view.Model.Should()
                .BeOfType<SignInStubViewModel>()
                .Which;

            model.UserId.Should().Be("user-two");
            model.ReturnUrl.Should().Be(returnUrl);

            model.Users.Select(user => user.Id)
                .Should()
                .Equal(new[] { "user-one", "user-two" });

            model.Users.Select(user => user.Email)
                .Should()
                .Equal(new[]
                {
                    "user.one@example.com",
                    "user.two@example.com"
                });
        }

        [TestCase(null, "LOCAL")]
        [TestCase("false", "LOCAL")]
        [TestCase("true", "PRD")]
        [TestCase("true", "pRd")]
        public void Then_The_Page_Is_Unavailable_When_Stub_Authentication_Is_Disabled(
            string? stubAuth,
            string environment)
        {
            // Arrange
            _configuration["StubAuth"] = stubAuth;
            _configuration["ResourceEnvironmentName"] = environment;

            // Act
            var result = _sut.SignInStub();

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public void Then_Only_The_Selected_Users_Organisations_And_Roles_Are_Shown()
        {
            // Arrange
            var organisationAId = Guid.Parse(
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            var organisationBId = Guid.Parse(
                "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

            _stubConfiguration.Users[0].Organisations.Add(
                new DfESignInStubOrganisation
                {
                    Id = organisationAId,
                    Name = "Organisation A",
                    UkPrn = 10000001
                });

            _stubConfiguration.Users[0].Organisations.Add(
                new DfESignInStubOrganisation
                {
                    Id = organisationBId,
                    Name = "Organisation B",
                    UkPrn = 10000002,
                    Roles = new List<Role>
                    {
                new Role
                {
                    Id = Guid.Parse(
                        "33333333-3333-3333-3333-333333333333"),
                    Name = "Administrator",
                    Code = "Administrator",
                    NumericId = 1,
                    Status = new Status { Id = 1 }
                }
                    }
                });

            _stubConfiguration.Users[1].Organisations.Add(
                new DfESignInStubOrganisation
                {
                    Id = organisationBId,
                    Name = "Organisation B",
                    UkPrn = 10000002,
                    Roles = new List<Role>
                    {
                new Role
                {
                    Id = Guid.Parse(
                        "44444444-4444-4444-4444-444444444444"),
                    Name = "Read only",
                    Code = "ReadOnly",
                    NumericId = 2,
                    Status = new Status { Id = 1 }
                }
                    }
                });

            var returnUrl = "/requested-page";

            // Act
            var result = _sut.SelectOrganisation("user-two", returnUrl);

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Which;

            view.ViewName.Should().Be("SelectOrganisation");

            var model = view.Model.Should()
                .BeOfType<SelectOrganisationStubViewModel>()
                .Which;

            model.UserId.Should().Be("user-two");
            model.Email.Should().Be("user.two@example.com");
            model.ReturnUrl.Should().Be(returnUrl);
            model.OrganisationId.Should().BeNull();

            model.Organisations.Should().ContainSingle();

            var organisation = model.Organisations[0];

            organisation.Id.Should().Be(organisationBId);
            organisation.Name.Should().Be("Organisation B");
            organisation.UkPrn.Should().Be(10000002);
            organisation.Roles.Should().ContainSingle();
            organisation.Roles[0].Code.Should().Be("ReadOnly");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("unknown-user")]
        public void Then_An_Invalid_User_Returns_NotFound(string? userId)
        {
            // Arrange
            var returnUrl = "/requested-page";

            // Act
            var result = _sut.SelectOrganisation(userId, returnUrl);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestCase(null, "LOCAL")]
        [TestCase("false", "LOCAL")]
        [TestCase("true", "PRD")]
        [TestCase("true", "pRd")]
        public void Then_The_Organisation_Page_Is_Unavailable_When_Stub_Authentication_Is_Disabled(
            string? stubAuth,
            string environment)
        {
            // Arrange
            _configuration["StubAuth"] = stubAuth;
            _configuration["ResourceEnvironmentName"] = environment;

            // Act
            var result = _sut.SelectOrganisation("user-two");

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}