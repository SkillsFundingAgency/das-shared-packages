using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SFA.DAS.DfESignIn.Auth.Api.Models;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Constants;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Services;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Services
{
    public class WhenGettingStubSignInClaims
    {
        private DfESignInStubConfiguration _stubConfiguration = null!;
        private Mock<ICustomServiceRole> _customServiceRole = null!;
        private DfESignInStubService _sut = null!;
        private Guid _organisationId;
        private Role _activeRole = null!;

        [SetUp]
        public void SetUp()
        {
            _organisationId = Guid.NewGuid();

            _activeRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Administrator",
                Code = "ADMIN",
                NumericId = 1,
                Status = new Status
                {
                    Id = (int)RoleStatus.Active
                }
            };

            _stubConfiguration = new DfESignInStubConfiguration
            {
                Users = new List<DfESignInStubUser>
                {
                    new DfESignInStubUser
                    {
                        Id = "user-one",
                        Email = "user.one@example.com",
                        GivenName = "User",
                        FamilyName = "One",
                        Organisations = new List<DfESignInStubOrganisation>
                        {
                            new DfESignInStubOrganisation
                            {
                                Id = _organisationId,
                                Name = "Organisation A",
                                UkPrn = 10000001,
                                Roles = new List<Role>
                                {
                                    _activeRole,
                                    new Role
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Inactive role",
                                        Code = "INACTIVE",
                                        NumericId = 2,
                                        Status = new Status
                                        {
                                            Id = (int)RoleStatus.InActive
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new DfESignInStubUser
                    {
                        Id = "user-two",
                        Email = "user.two@example.com",
                        GivenName = "User",
                        FamilyName = "Two",
                        Organisations = new List<DfESignInStubOrganisation>
                        {
                            new DfESignInStubOrganisation
                            {
                                Id = _organisationId,
                                Name = "Organisation A",
                                UkPrn = 10000001,
                                Roles = new List<Role>
                                {
                                    new Role
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Read only",
                                        Code = "READ",
                                        NumericId = 3,
                                        Status = new Status
                                        {
                                            Id = (int)RoleStatus.Active
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _customServiceRole = new Mock<ICustomServiceRole>();

            _customServiceRole
                .Setup(role => role.RoleClaimType)
                .Returns(ClaimTypes.Role);

            _customServiceRole
                .Setup(role => role.RoleValueType)
                .Returns(CustomServiceRoleValueType.Name);

            _sut = new DfESignInStubService(
                Options.Create(_stubConfiguration),
                Options.Create(new DfEOidcConfiguration
                {
                    ClientId = "assessor-client"
                }),
                _customServiceRole.Object);
        }

        [Test]
        public void Then_The_Configured_User_And_Organisation_Claims_Are_Returned()
        {
            // Arrange
            const string userId = "user-one";

            // Act
            var actual = _sut.GetStubSignInClaims(
                userId,
                _organisationId);

            // Assert
            actual.Should().NotBeNull();

            actual.Identity!.IsAuthenticated.Should().BeTrue();

            actual.Identity.AuthenticationType.Should().Be(
                CookieAuthenticationDefaults.AuthenticationScheme);

            actual.FindFirst(ClaimName.Sub)!.Value
                .Should().Be(userId);

            actual.FindFirst(ClaimName.Email)!.Value
                .Should().Be("user.one@example.com");

            actual.FindFirst(ClaimName.GivenName)!.Value
                .Should().Be("User");

            actual.FindFirst(ClaimName.FamilyName)!.Value
                .Should().Be("One");

            actual.Identity.Name.Should().Be("User One");

            actual.FindFirst(CustomClaimsIdentity.DisplayName)!.Value
                .Should().Be("User One");

            actual.FindFirst(CustomClaimsIdentity.UkPrn)!.Value
                .Should().Be("10000001");

            var organisation = JObject.Parse(
                actual.FindFirst(ClaimName.Organisation)!.Value);

            organisation.Properties()
                .Select(property => property.Name)
                .Should()
                .BeEquivalentTo(new[] { "id", "name", "ukprn" });

            Guid.Parse(organisation.Value<string>("id")!)
                .Should().Be(_organisationId);

            organisation.Value<string>("name")
                .Should().Be("Organisation A");

            organisation["ukprn"]!.Type
                .Should().Be(JTokenType.String);

            organisation.Value<string>("ukprn")
                .Should().Be("10000001");
        }

        [Test]
        public void Then_Only_Active_Roles_Are_Mapped()
        {
            // Arrange
            var expectedClaims = new Dictionary<string, string>
            {
                { ClaimName.RoleCode, _activeRole.Code },
                { ClaimName.RoleId, _activeRole.Id.ToString() },
                { ClaimName.RoleName, _activeRole.Name },
                { ClaimName.RoleNumericId, _activeRole.NumericId.ToString() }
            };

            // Act
            var actual = _sut.GetStubSignInClaims(
                "user-one",
                _organisationId);

            // Assert
            foreach (var expectedClaim in expectedClaims)
            {
                var claim = actual.FindAll(expectedClaim.Key)
                    .Should().ContainSingle().Subject;

                claim.Value.Should().Be(expectedClaim.Value);
                claim.Issuer.Should().Be("assessor-client");
                claim.ValueType.Should().Be(ClaimTypes.Role);
            }

            actual.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .Should()
                .BeEquivalentTo(new[] { "Administrator" });
        }

        [TestCase(
            CustomServiceRoleValueType.Name,
            "custom-role",
            "Administrator")]
        [TestCase(
            CustomServiceRoleValueType.Code,
            "custom-role",
            "ADMIN")]
        [TestCase(
            CustomServiceRoleValueType.Name,
            null,
            "Administrator")]
        [TestCase(
            CustomServiceRoleValueType.Code,
            null,
            "ADMIN")]
        public void Then_The_Configured_Service_Role_Mapping_Is_Used(
            CustomServiceRoleValueType roleValueType,
            string? roleClaimType,
            string expectedValue)
        {
            // Arrange
            _customServiceRole
                .Setup(role => role.RoleClaimType)
                .Returns(roleClaimType!);

            _customServiceRole
                .Setup(role => role.RoleValueType)
                .Returns(roleValueType);

            // Act
            var actual = _sut.GetStubSignInClaims(
                "user-one",
                _organisationId);

            // Assert
            actual.FindAll(roleClaimType ?? CustomClaimsIdentity.Service)
                .Select(claim => claim.Value)
                .Should()
                .BeEquivalentTo(new[] { expectedValue });
        }

        [Test]
        public void Then_Roles_Belong_To_The_Selected_User_Organisation_Membership()
        {
            // Arrange
            const string userId = "user-two";

            // Act
            var actual = _sut.GetStubSignInClaims(
                userId,
                _organisationId);

            // Assert
            actual.FindFirst(ClaimName.Sub)!.Value.Should().Be(userId);

            actual.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .Should()
                .BeEquivalentTo(new[] { "Read only" });

            actual.FindAll(ClaimName.RoleCode)
                .Select(claim => claim.Value)
                .Should()
                .BeEquivalentTo(new[] { "READ" });
        }

        [Test]
        public void Then_A_Missing_UkPrn_Uses_Zero()
        {
            // Arrange
            _stubConfiguration.Users[0].Organisations[0].UkPrn = null;

            // Act
            var actual = _sut.GetStubSignInClaims(
                "user-one",
                _organisationId);

            // Assert
            actual.FindFirst(CustomClaimsIdentity.UkPrn)!.Value
                .Should().Be("0");

            var organisation = JObject.Parse(
                actual.FindFirst(ClaimName.Organisation)!.Value);

            organisation.Value<int?>("ukprn").Should().BeNull();
        }

        [Test]
        public void Then_A_User_With_No_Roles_Can_Still_Have_Identity_Claims()
        {
            // Arrange
            _stubConfiguration.Users[0].Organisations[0].Roles.Clear();

            // Act
            var actual = _sut.GetStubSignInClaims(
                "user-one",
                _organisationId);

            // Assert
            actual.Identity!.IsAuthenticated.Should().BeTrue();
            actual.FindFirst(ClaimName.Sub)!.Value.Should().Be("user-one");
            actual.FindAll(ClaimTypes.Role).Should().BeEmpty();
            actual.FindAll(ClaimName.RoleCode).Should().BeEmpty();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("unknown-user")]
        public void Then_An_Invalid_User_Returns_Null(string? userId)
        {
            // Arrange
            var organisationId = _organisationId;

            // Act
            var actual = _sut.GetStubSignInClaims(
                userId!,
                organisationId);

            // Assert
            actual.Should().BeNull();
        }

        [Test]
        public void Then_An_Organisation_Outside_The_User_Membership_Returns_Null()
        {
            // Arrange
            var otherOrganisationId = Guid.NewGuid();

            _stubConfiguration.Users[1].Organisations.Add(
                new DfESignInStubOrganisation
                {
                    Id = otherOrganisationId,
                    Name = "Organisation B"
                });

            // Act
            var actual = _sut.GetStubSignInClaims(
                "user-one",
                otherOrganisationId);

            // Assert
            actual.Should().BeNull();
        }
    }
}