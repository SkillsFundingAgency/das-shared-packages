using AutoFixture;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Services;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Services
{
    [TestFixture]
    public class DfESignInServiceTest
    {
        private MockRepository? _mockRepository;
        private IOptions<DfEOidcConfiguration>? _configuration;
        private DfESignInService? _dfESignInService;

        private Mock<ITokenDataSerializer>? _mockTokenDataSerializer;
        private Mock<ITokenEncoder>? _mockTokenEncoder;
        private Mock<IJsonWebAlgorithm>? _mockJsonWebAlgorithm;
        private Mock<ITokenData>? _mockTokenData;
        private Mock<IApiHelper>? _mockApiHelper;


        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _configuration = Options.Create(new DfEOidcConfiguration());
            _mockTokenDataSerializer = _mockRepository.Create<ITokenDataSerializer>();
            _mockTokenEncoder = _mockRepository.Create<ITokenEncoder>();
            _mockJsonWebAlgorithm = _mockRepository.Create<IJsonWebAlgorithm>();
            _mockTokenData = _mockRepository.Create<ITokenData>();
            _mockApiHelper = _mockRepository.Create<IApiHelper>();
        }

        [TestCase("")]
        [TestCase(null)]
        public void GetDestinationUrl_ThrowsException_Missing_UserId(string userId)
        {
            var fixture = new Fixture();

            // arrange
            _dfESignInService = GetDfESignInService();

            if (_configuration != null)
            {
                _configuration.Value.APIServiceId = fixture.Create<string>()[.. 5];
                _configuration.Value.APIServiceUrl = fixture.Create<string>()[..5];
            }

            var orgId = fixture.Create<string>()[..5];

            //sut
            Assert.Throws<ArgumentNullException>(() => _dfESignInService.GetDestinationUrl(userId, orgId));
        }

        [TestCase("")]
        [TestCase(null)]
        public void GetDestinationUrl_ThrowsException_Missing_OrgId(string orgId)
        {
            var fixture = new Fixture();

            // arrange
            _dfESignInService = GetDfESignInService();

            if (_configuration != null)
            {
                _configuration.Value.APIServiceId = fixture.Create<string>()[..5];
                _configuration.Value.APIServiceUrl = fixture.Create<string>()[..5];
            }

            var userId = fixture.Create<string>()[..5];

            // sut
            Assert.Throws<ArgumentNullException>(() => _dfESignInService.GetDestinationUrl(userId, orgId));
        }

        [TestCase("")]
        [TestCase(null)]
        public void GetDestinationUrl_ThrowsException_Missing_ServiceId(string serviceId)
        {
            var fixture = new Fixture();

            // arrange
            _dfESignInService = GetDfESignInService();

            if (_configuration != null)
            {
                _configuration.Value.APIServiceId = serviceId;
                _configuration.Value.APIServiceUrl = fixture.Create<string>()[..5];
            }

            var userId = fixture.Create<string>()[..5];
            var orgId = fixture.Create<string>()[..5];

            // sut
            Assert.Throws<MemberAccessException>(() => _dfESignInService.GetDestinationUrl(userId, orgId));
        }

        [TestCase("")]
        [TestCase(null)]
        public void GetDestinationUrl_ThrowsException_Missing_ServiceUrl(string serviceUrl)
        {
            var fixture = new Fixture();

            // arrange
            _dfESignInService = GetDfESignInService();

            if (_configuration != null)
            {
                _configuration.Value.APIServiceId = fixture.Create<string>()[..5];
                _configuration.Value.APIServiceUrl = serviceUrl;
            }

            var userId = fixture.Create<string>()[..5];
            var orgId = fixture.Create<string>()[..5];

            // sut
            Assert.Throws<MemberAccessException>(() => _dfESignInService.GetDestinationUrl(userId, orgId));
        }

        private DfESignInService GetDfESignInService()
        {
            return new DfESignInService(
                _configuration,
                _mockTokenDataSerializer?.Object,
                _mockTokenEncoder?.Object,
                _mockJsonWebAlgorithm?.Object,
                _mockTokenData?.Object,
                _mockApiHelper?.Object);
        }
    }
}
