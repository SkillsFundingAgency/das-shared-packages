using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.Testing.Builders;
using System.Reflection;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Client
{
    [TestFixture]
    public class DfESignInClientTests
    {
        private MockRepository _mockRepository;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<HttpClient> _mockHttpClient;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockConfiguration = _mockRepository.Create<IConfiguration>();
            _mockHttpClient = _mockRepository.Create<HttpClient>();
        }

        [Test]
        public void CreateDfESignInClient_ThrowsExceptionWhenNoSecretKey()
        {
            var fixture = new Fixture();

            var factory = new DfESignInClientFactory(
                _mockConfiguration.Object);

            var mockIConfigurationSection = new Mock<IConfigurationSection>();
            mockIConfigurationSection.Setup(x => x.GetSection("DfEOidcConfiguration")).Returns(mockIConfigurationSection.Object);

            string userId = fixture.Create<string>();
            string organizationId = fixture.Create<string>();

            _mockRepository.VerifyAll();
            Assert.Throws<Exception>(() => factory.CreateDfESignInClient(userId, organizationId));
        }

        [Test]
        public void CreateDfESignInClient_ThrowsExceptionWhenNoSecretKeysdfsdfsdfs()
        {
            var fixture = new Fixture();

            var factory = new DfESignInClientFactory(
                _mockConfiguration.Object);

            var mockIConfigurationSection = new Mock<IConfigurationSection>();
            mockIConfigurationSection.Setup(x => x.GetSection("DfEOidcConfiguration")).Returns(mockIConfigurationSection.Object);

            string userId = fixture.Create<string>();
            string organizationId = fixture.Create<string>();

            _mockRepository.VerifyAll();
            Assert.Throws<Exception>(() => factory.CreateDfESignInClient(userId, organizationId));
        }

        [Test]
        public void CreateDfESignInClient_TargetAddressThrowsException_MissingServiceUrl()
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfESignInClient(
                _mockHttpClient.Object);

            client.OrganisationId = fixture.Create<string>().Substring(0, 5);
            client.ServiceId = fixture.Create<string>().Substring(0, 5);
            client.UserId = fixture.Create<string>().Substring(0, 5);

            Assert.Throws<MemberAccessException>(delegate
            {
                object result = client.TargetAddress;
            });
        }


        [Test]
        public void CreateDfESignInClient_TargetAddressThrowsException_MissingOrganisationId()
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfESignInClient(
                _mockHttpClient.Object);

            client.ServiceId = fixture.Create<string>().Substring(0, 5);
            client.ServiceUrl = fixture.Create<Uri>().ToString();
            client.UserId = fixture.Create<string>().Substring(0, 5);

            Assert.Throws<MemberAccessException>(delegate
            {
                object result = client.TargetAddress;
            });
        }


        [Test]
        public void CreateDfESignInClient_TargetAddressThrowsException_MissingServiceId()
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfESignInClient(
                _mockHttpClient.Object);

             client.OrganisationId = fixture.Create<string>().Substring(0, 5);
            client.ServiceUrl = fixture.Create<Uri>().ToString();
            client.UserId = fixture.Create<string>().Substring(0, 5);

            Assert.Throws<MemberAccessException>(delegate
            {
                object result = client.TargetAddress;
            });
        }

        [Test]
        public void CreateDfESignInClient_TargetAddressThrowsException_MissingUserId()
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfESignInClient(
                _mockHttpClient.Object);

            client.OrganisationId = fixture.Create<string>().Substring(0, 5);
            client.ServiceId = fixture.Create<string>().Substring(0, 5);
            client.ServiceUrl = fixture.Create<Uri>().ToString();

            Assert.Throws<MemberAccessException>(delegate
            {
                object result = client.TargetAddress;
            });
        }

        [Test]
        public void CreateDfESignInClient_TargetAddressThrowsException_UrlFormatWrong()
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfESignInClient(
                _mockHttpClient.Object);

            client.OrganisationId = fixture.Create<string>().Substring(0, 5);
            client.ServiceId = fixture.Create<string>().Substring(0, 5);
            client.UserId = fixture.Create<string>().Substring(0, 5);
            client.ServiceUrl = "";

            Assert.Throws<MemberAccessException>(delegate
            {
                object result = client.TargetAddress;
            });
        }


    }
}
