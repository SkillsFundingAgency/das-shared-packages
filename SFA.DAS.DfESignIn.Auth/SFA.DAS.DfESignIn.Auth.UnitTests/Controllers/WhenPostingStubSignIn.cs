using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Constants;
using SFA.DAS.DfESignIn.Auth.Controllers;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Models.Stub;
using System.Security.Claims;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Controllers
{
    public class WhenPostingStubSignIn
    {
        private IConfigurationRoot _configuration = null!;
        private DfESignInStubConfiguration _stubConfiguration = null!;
        private Mock<IDfESignInStubService> _stubService = null!;
        private Mock<IAuthenticationService> _authenticationService = null!;
        private Mock<IUrlHelper> _urlHelper = null!;
        private Mock<ICustomClaims> _customClaims = null!;
        private ServiceProvider _serviceProvider = null!;
        private ClaimsPrincipal _principal = null!;
        private DfESignInStubController _sut = null!;
        private Guid _organisationId;
        private Guid _otherOrganisationId;

        [SetUp]
        public void SetUp()
        {
            _organisationId = Guid.NewGuid();
            _otherOrganisationId = Guid.NewGuid();

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "StubAuth", "true" },
                    { "ResourceEnvironmentName", "LOCAL" }
                })
                .Build();

            _stubConfiguration = new DfESignInStubConfiguration
            {
                Users = new List<DfESignInStubUser>
                {
                    new DfESignInStubUser
                    {
                        Id = "user-one",
                        Email = "user.one@example.com",
                        Organisations = new List<DfESignInStubOrganisation>
                        {
                            new DfESignInStubOrganisation
                            {
                                Id = _organisationId,
                                Name = "Organisation A"
                            }
                        }
                    },
                    new DfESignInStubUser
                    {
                        Id = "user-two",
                        Email = "user.two@example.com",
                        Organisations = new List<DfESignInStubOrganisation>
                        {
                            new DfESignInStubOrganisation
                            {
                                Id = _otherOrganisationId,
                                Name = "Organisation B"
                            }
                        }
                    }
                }
            };

            _principal = new ClaimsPrincipal(new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimName.Sub, "user-one")
                },
                CookieAuthenticationDefaults.AuthenticationScheme));

            _stubService = new Mock<IDfESignInStubService>();

            _stubService
                .Setup(service => service.GetStubSignInClaims(
                    "user-one",
                    _organisationId))
                .Returns(_principal);

            _authenticationService = new Mock<IAuthenticationService>();

            _authenticationService
                .Setup(service => service.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            _customClaims = new Mock<ICustomClaims>();

            _customClaims
                .Setup(handler => handler.GetClaimsAsync(
                    It.IsAny<TokenValidatedContext>()))
                .ReturnsAsync(Array.Empty<Claim>());

            _serviceProvider = new ServiceCollection()
                .AddSingleton<IAuthenticationService>(_authenticationService.Object)
                .AddScoped<ICustomClaims>(_ => _customClaims.Object)
                .BuildServiceProvider();

            _urlHelper = new Mock<IUrlHelper>();

            _urlHelper
                .Setup(helper => helper.IsLocalUrl("/requested-page"))
                .Returns(true);

            _urlHelper
                .Setup(helper => helper.Content("~/"))
                .Returns("/");

            _sut = new DfESignInStubController(
                _configuration,
                Options.Create(_stubConfiguration),
                _stubService.Object,
                Options.Create(new DfEOidcConfiguration
                {
                    ClientId = "assessor-client"
                }))
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = _serviceProvider
                    }
                },
                Url = _urlHelper.Object
            };

            _sut.TempData = new TempDataDictionary(
                _sut.HttpContext,
                Mock.Of<ITempDataProvider>());
        }

        [TearDown]
        public void TearDown()
        {
            _sut.Dispose();
            _serviceProvider.Dispose();
            (_configuration as IDisposable)?.Dispose();
        }

        [Test]
        public async Task Then_The_User_Is_Signed_In_And_Redirected()
        {
            // Arrange
            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                OrganisationId = _organisationId,
                ReturnUrl = "/requested-page"
            };

            // Act
            var actual = await _sut.SignIn(model);

            // Assert
            actual.Should().BeOfType<LocalRedirectResult>()
                .Which.Url.Should().Be("/requested-page");

            _stubService.Verify(
                service => service.GetStubSignInClaims(
                    "user-one",
                    _organisationId),
                Times.Once);

            _authenticationService.Verify(
                service => service.SignInAsync(
                    _sut.HttpContext,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    _principal,
                    It.IsAny<AuthenticationProperties>()),
                Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("https://external.example.com")]
        [TestCase("//external.example.com")]
        public async Task Then_An_Invalid_Return_Url_Redirects_To_The_Application_Root(
            string? returnUrl)
        {
            // Arrange
            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                OrganisationId = _organisationId,
                ReturnUrl = returnUrl!
            };

            // Act
            var actual = await _sut.SignIn(model);

            // Assert
            actual.Should().BeOfType<LocalRedirectResult>()
                .Which.Url.Should().Be("/");

            _authenticationService.Verify(
                service => service.SignInAsync(
                    _sut.HttpContext,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    _principal,
                    It.IsAny<AuthenticationProperties>()),
                Times.Once);
        }

        [Test]
        public async Task Then_A_Missing_Organisation_Redisplays_The_Chooser()
        {
            // Arrange
            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                ReturnUrl = "/requested-page"
            };

            // Act
            var actual = await _sut.SignIn(model);

            // Assert
            var view = actual.Should().BeOfType<ViewResult>().Subject;
            view.ViewName.Should().Be("SelectOrganisation");

            var returnedModel = view.Model.Should()
                .BeOfType<SelectOrganisationStubViewModel>().Subject;

            returnedModel.Email.Should().Be("user.one@example.com");
            returnedModel.ReturnUrl.Should().Be("/requested-page");
            returnedModel.Organisations.Should()
                .BeSameAs(_stubConfiguration.Users[0].Organisations);

            _sut.ModelState.IsValid.Should().BeFalse();
            _sut.ModelState.Should().ContainKey(
                nameof(SelectOrganisationStubViewModel.OrganisationId));

            _stubService.VerifyNoOtherCalls();
            _authenticationService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_An_Organisation_Belonging_To_Another_User_Is_Rejected()
        {
            // Arrange
            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                OrganisationId = _otherOrganisationId,
                Email = "submitted@example.com",
                Organisations = _stubConfiguration.Users[1].Organisations
            };

            // Act
            var actual = await _sut.SignIn(model);

            // Assert
            var view = actual.Should().BeOfType<ViewResult>().Subject;
            view.ViewName.Should().Be("SelectOrganisation");

            var returnedModel = view.Model.Should()
                .BeOfType<SelectOrganisationStubViewModel>().Subject;

            returnedModel.Email.Should().Be("user.one@example.com");
            returnedModel.Organisations.Should()
                .BeSameAs(_stubConfiguration.Users[0].Organisations);

            _sut.ModelState.IsValid.Should().BeFalse();

            _stubService.VerifyNoOtherCalls();
            _authenticationService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_Existing_ModelState_Errors_Prevent_Sign_In()
        {
            // Arrange
            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                OrganisationId = _organisationId
            };

            _sut.ModelState.AddModelError(
                nameof(model.OrganisationId),
                "Invalid organisation.");

            // Act
            var actual = await _sut.SignIn(model);

            // Assert
            actual.Should().BeOfType<ViewResult>()
                .Which.ViewName.Should().Be("SelectOrganisation");

            _stubService.VerifyNoOtherCalls();
            _authenticationService.VerifyNoOtherCalls();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("unknown-user")]
        public async Task Then_An_Invalid_User_Returns_NotFound(string? userId)
        {
            // Arrange
            var model = new SelectOrganisationStubViewModel
            {
                UserId = userId!,
                OrganisationId = _organisationId
            };

            // Act
            var actual = await _sut.SignIn(model);

            // Assert
            actual.Should().BeOfType<NotFoundResult>();

            _stubService.VerifyNoOtherCalls();
            _authenticationService.VerifyNoOtherCalls();
        }

        [TestCase(null, "LOCAL")]
        [TestCase("false", "LOCAL")]
        [TestCase("true", "PRD")]
        [TestCase("true", "pRd")]
        public async Task Then_An_Unavailable_Stub_Returns_NotFound(
            string? stubAuth,
            string environment)
        {
            // Arrange
            _configuration["StubAuth"] = stubAuth;
            _configuration["ResourceEnvironmentName"] = environment;

            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                OrganisationId = _organisationId
            };

            // Act
            var actual = await _sut.SignIn(model);

            // Assert
            actual.Should().BeOfType<NotFoundResult>();

            _stubService.VerifyNoOtherCalls();
            _authenticationService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_A_Missing_Principal_Prevents_Sign_In()
        {
            // Arrange
            _stubService
                .Setup(service => service.GetStubSignInClaims(
                    "user-one",
                    _organisationId))
                .Returns((ClaimsPrincipal)null!);

            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                OrganisationId = _organisationId
            };

            // Act
            var actual = await _sut.SignIn(model);

            // Assert
            actual.Should().BeOfType<NotFoundResult>();

            _authenticationService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_Custom_Claims_Are_Added_Before_Signing_In()
        {
            // Arrange
            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                OrganisationId = _organisationId,
                ReturnUrl = "/requested-page"
            };

            _customClaims
                .Setup(handler => handler.GetClaimsAsync(
                    It.IsAny<TokenValidatedContext>()))
                .ReturnsAsync(new[]
                {
                    new Claim("application-user-id", "application-user-one")
                });

            var claimWasPresentAtSignIn = false;

            _authenticationService
                .Setup(service => service.SignInAsync(
                    _sut.HttpContext,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    _principal,
                    It.IsAny<AuthenticationProperties>()))
                .Callback(() =>
                {
                    claimWasPresentAtSignIn = _principal.HasClaim(
                        "application-user-id",
                        "application-user-one");
                })
                .Returns(Task.CompletedTask);

            // Act
            var actual = await _sut.SignIn(model);

            // Assert
            claimWasPresentAtSignIn.Should().BeTrue();

            actual.Should().BeOfType<LocalRedirectResult>()
                .Which.Url.Should().Be("/requested-page");

            _customClaims.Verify(
                handler => handler.GetClaimsAsync(
                    It.Is<TokenValidatedContext>(context =>
                        context.Principal == _principal &&
                        context.HttpContext == _sut.HttpContext &&
                        context.Options.ClientId == "assessor-client")),
                Times.Once);

            _authenticationService.Verify(
                service => service.SignInAsync(
                    _sut.HttpContext,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    _principal,
                    It.IsAny<AuthenticationProperties>()),
                Times.Once);
        }

        [Test]
        public async Task Then_A_Custom_Claims_Redirect_Is_Preserved_Without_Signing_In()
        {
            // Arrange
            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                OrganisationId = _organisationId
            };

            _customClaims
                .Setup(handler => handler.GetClaimsAsync(
                    It.IsAny<TokenValidatedContext>()))
                .Callback<TokenValidatedContext>(context =>
                {
                    context.Response.Redirect("/access-denied");
                    context.HandleResponse();
                })
                .ReturnsAsync(Array.Empty<Claim>());

            // Act
            var actual = await _sut.SignIn(model);

            // Assert
            actual.Should().BeOfType<EmptyResult>();

            _sut.Response.StatusCode.Should()
                .Be(StatusCodes.Status302Found);

            _sut.Response.Headers["Location"].ToString()
                .Should().Be("/access-denied");

            _authenticationService.VerifyNoOtherCalls();
        }

        [TestCase("failed")]
        [TestCase("skipped")]
        public async Task Then_A_Stopped_Custom_Claims_Result_Prevents_Sign_In(
            string outcome)
        {
            // Arrange
            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                OrganisationId = _organisationId
            };

            _customClaims
                .Setup(handler => handler.GetClaimsAsync(
                    It.IsAny<TokenValidatedContext>()))
                .Callback<TokenValidatedContext>(context =>
                {
                    if (outcome == "failed")
                    {
                        context.Fail("Access denied.");
                    }
                    else
                    {
                        context.SkipHandler();
                    }
                })
                .ReturnsAsync(Array.Empty<Claim>());

            // Act
            Func<Task> act = () => _sut.SignIn(model);

            // Assert
            var exception = await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage(
                    "The custom claims handler stopped stub sign-in.");

            if (outcome == "failed")
            {
                exception.Which.InnerException.Should().NotBeNull();
                exception.Which.InnerException!.Message
                    .Should().Be("Access denied.");
            }
            else
            {
                exception.Which.InnerException.Should().BeNull();
            }

            _authenticationService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_A_Custom_Claims_Exception_Prevents_Sign_In()
        {
            // Arrange
            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                OrganisationId = _organisationId
            };

            _customClaims
                .Setup(handler => handler.GetClaimsAsync(
                    It.IsAny<TokenValidatedContext>()))
                .ThrowsAsync(new InvalidOperationException(
                    "User creation failed."));

            // Act
            Func<Task> act = () => _sut.SignIn(model);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("User creation failed.");

            _authenticationService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_The_Replacement_Principal_Is_Used_For_Sign_In()
        {
            // Arrange
            var model = new SelectOrganisationStubViewModel
            {
                UserId = "user-one",
                OrganisationId = _organisationId,
                ReturnUrl = "/requested-page"
            };

            var replacementPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimName.Sub, "user-one")
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme));

            _customClaims
                .Setup(handler => handler.GetClaimsAsync(
                    It.IsAny<TokenValidatedContext>()))
                .Callback<TokenValidatedContext>(context =>
                {
                    context.Principal = replacementPrincipal;
                })
                .ReturnsAsync(new[]
                {
                    new Claim("application-user-id", "application-user-one")
                });

            // Act
            var actual = await _sut.SignIn(model);

            // Assert
            actual.Should().BeOfType<LocalRedirectResult>()
                .Which.Url.Should().Be("/requested-page");

            replacementPrincipal.FindFirst("application-user-id")!.Value
                .Should().Be("application-user-one");

            _authenticationService.Verify(
                service => service.SignInAsync(
                    _sut.HttpContext,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    replacementPrincipal,
                    It.IsAny<AuthenticationProperties>()),
                Times.Once);

            _authenticationService.Verify(
                service => service.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    _principal,
                    It.IsAny<AuthenticationProperties>()),
                Times.Never);
        }
    }
}