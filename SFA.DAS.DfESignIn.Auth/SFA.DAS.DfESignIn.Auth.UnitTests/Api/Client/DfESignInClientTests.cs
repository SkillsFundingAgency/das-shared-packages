using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.Testing.Builders;
using System.Reflection;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Client
{
    [TestFixture]
    public class DfESignInClientTests
    {
        private MockRepository _mockRepository;
        private Mock<DfEOidcConfiguration> _mockConfiguration;
        private Mock<HttpClient> _mockHttpClient;

        private Mock<ITokenDataSerializer> _mockTokenDataSerializer;
        private Mock<ITokenEncoder> _mockTokenEncoder;
        private Mock<IJsonWebAlgorithm> _mockJsonWebAlgorithm;
        private Mock<ITokenData> _mockTokenData;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockConfiguration = _mockRepository.Create<DfEOidcConfiguration>();
            _mockHttpClient = _mockRepository.Create<HttpClient>();
            _mockTokenDataSerializer = _mockRepository.Create<ITokenDataSerializer>();
            _mockTokenEncoder = _mockRepository.Create<ITokenEncoder>();
            _mockJsonWebAlgorithm = _mockRepository.Create<IJsonWebAlgorithm>();
            _mockTokenData = _mockRepository.Create<ITokenData>();
        }

        [Test]
        public void TargetAddressThrowsException_Missing_ServiceUrl()
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfESignInClient(
                _mockHttpClient.Object)
            {
                OrganisationId = fixture.Create<string>()[..5],
                ServiceId = fixture.Create<string>()[..5],
                UserId = fixture.Create<string>()[..5]
            };

            Assert.Throws<MemberAccessException>(delegate
            {
                object result = client.TargetAddress;
            });
        }


        [Test]
        public void TargetAddressThrowsException_Missing_OrganisationId()
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfESignInClient(
                _mockHttpClient.Object);

            client.ServiceId = fixture.Create<string>()[..5];
            client.ServiceUrl = fixture.Create<Uri>().ToString();
            client.UserId = fixture.Create<string>()[..5];

            Assert.Throws<MemberAccessException>(delegate
            {
                object result = client.TargetAddress;
            });
        }


        [Test]
        public void TargetAddressThrowsException_Missing_ServiceId()
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfESignInClient(
                _mockHttpClient.Object);

            client.OrganisationId = fixture.Create<string>()[..5];
            client.ServiceUrl = fixture.Create<Uri>().ToString();
            client.UserId = fixture.Create<string>()[..5];

            Assert.Throws<MemberAccessException>(delegate
            {
                object result = client.TargetAddress;
            });
        }

        [Test]
        public void TargetAddressThrowsException_Missing_UserId()
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfESignInClient(
                _mockHttpClient.Object);

            client.OrganisationId = fixture.Create<string>()[..5];
            client.ServiceId = fixture.Create<string>()[..5];
            client.ServiceUrl = fixture.Create<Uri>().ToString();

            Assert.Throws<MemberAccessException>(delegate
            {
                object result = client.TargetAddress;
            });
        }

        [Test]
        public void TargetAddressThrowsException_NoUrlSupplied()
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfESignInClient(
                _mockHttpClient.Object);

            client.OrganisationId = fixture.Create<string>()[..5];
            client.ServiceId = fixture.Create<string>()[..5];
            client.UserId = fixture.Create<string>()[..5];
            client.ServiceUrl = "";

            Assert.Throws<MemberAccessException>(delegate
            {
                object result = client.TargetAddress;
            });
        }
    }
}
