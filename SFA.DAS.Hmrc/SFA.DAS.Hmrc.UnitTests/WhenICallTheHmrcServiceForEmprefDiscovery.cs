using System.Collections.Generic;
using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Client;
using HMRC.ESFA.Levy.Api.Types;
using Moq;
using NUnit.Framework;
using SFA.DAS.Hmrc.Configuration;
using SFA.DAS.Hmrc.Http;
using SFA.DAS.NLog.Logger;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.TokenService.Api.Types;

namespace SFA.DAS.Hmrc.UnitTests
{
    public class WhenICallTheHmrcServiceForEmprefDiscovery
    {
        private const string ExpectedEmpref = "123/AB12345";
        private const string ExpectedAuthToken = "789654321AGFVD";
        private readonly string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
        private readonly string ExpectedClientId = "654321";
        private readonly string ExpectedClientSecret = "my_secret";
        private readonly string ExpectedScope = "emp_ref";
        private Mock<IApprenticeshipLevyApiClient> _apprenticeshipLevyApiClient;
        private HmrcConfiguration _configuration;

        private HmrcService _hmrcService;
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private Mock<ITokenServiceApiClient> _tokenService;

        [SetUp]
        public void Arrange()
        {
            _configuration = new HmrcConfiguration
            {
                BaseUrl = ExpectedBaseUrl,
                ClientId = ExpectedClientId,
                Scope = ExpectedScope,
                ClientSecret = ExpectedClientSecret
            };

            _httpClientWrapper = new Mock<IHttpClientWrapper>();

            _apprenticeshipLevyApiClient = new Mock<IApprenticeshipLevyApiClient>();
            _apprenticeshipLevyApiClient.Setup(x => x.GetAllEmployers(It.IsAny<string>())).ReturnsAsync(new EmprefDiscovery { Emprefs = new List<string> { ExpectedEmpref } });

            _tokenService = new Mock<ITokenServiceApiClient>();
            _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAuthToken });
            _hmrcService = new HmrcService(_configuration, _httpClientWrapper.Object, _apprenticeshipLevyApiClient.Object, _tokenService.Object, new NoopExecutionPolicy(), null, null, new Mock<ILog>().Object);
        }

        [Test]
        public async Task ThenTheCorrectUrlIsUsedToDiscoverTheEmpref()
        {
            //Arrange
            var authToken = "123FGV";

            //Act
            await _hmrcService.DiscoverEmpref(authToken);

            //Assert
            _apprenticeshipLevyApiClient.Verify(x => x.GetAllEmployers(authToken), Times.Once);
        }

        [Test]
        public async Task ThenTheEmprefIsReturned()
        {
            //Arrange
            var authToken = "123FGV";

            //Act
            var actual = await _hmrcService.DiscoverEmpref(authToken);

            //Assert
            Assert.AreEqual(ExpectedEmpref, actual);
        }
    }
}