using System.Threading.Tasks;
using System.Web;
using HMRC.ESFA.Levy.Api.Client;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Hmrc.Configuration;
using SFA.DAS.Hmrc.Http;
using SFA.DAS.Hmrc.Models;
using SFA.DAS.NLog.Logger;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.TokenService.Api.Types;

namespace SFA.DAS.Hmrc.UnitTests
{
    public class WhenICallTheHmrcServiceForAuthentication
    {
        private const string ExpectedAccessCode = "789654321AGFVD";
        private readonly string ExpectedBaseUrl = "http://hmrcbase.gov.uk/";
        private readonly string ExpectedClientId = "654321";
        private readonly string ExpectedClientSecret = "my_secret";
        private readonly string ExpectedOgdClientId = "123456789";
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
                OgdClientId = ExpectedOgdClientId,
                Scope = ExpectedScope,
                ClientSecret = ExpectedClientSecret
            };

            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _httpClientWrapper.Setup(x => x.SendMessage(It.IsAny<object>(), It.IsAny<string>())).ReturnsAsync(JsonConvert.SerializeObject(new HmrcTokenResponse()));

            _tokenService = new Mock<ITokenServiceApiClient>();
            _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAccessCode });

            _apprenticeshipLevyApiClient = new Mock<IApprenticeshipLevyApiClient>();

            _hmrcService = new HmrcService(_configuration, _httpClientWrapper.Object,
                _apprenticeshipLevyApiClient.Object, _tokenService.Object, new NoopExecutionPolicy(), null, null, new Mock<ILog>().Object);
        }

        [Test]
        public void ThenTheAuthUrlIsGeneratedFromTheStoredConfigValues()
        {
            //Arrange
            var redirectUrl = "http://mytestUrl.to.redirectto?a=564kjg";
            var urlFriendlyRedirectUrl = HttpUtility.UrlEncode(redirectUrl);

            //Assert
            var actual = _hmrcService.GenerateAuthRedirectUrl(redirectUrl);

            //Assert
            Assert.AreEqual($"{ExpectedBaseUrl}oauth/authorize?response_type=code&client_id={ExpectedClientId}&scope={ExpectedScope}&redirect_uri={urlFriendlyRedirectUrl}", actual);
        }

        [Test]
        public async Task ThenTheCodeIsExchangedForTheAccessToken()
        {
            //Arrange
            var code = "ghj567";
            var redirectUrl = "http://mytestUrl.to.redirectto?a=564kjg";


            //Act
            var actual = await _hmrcService.GetAuthenticationToken(redirectUrl, code);

            //Assert
            _httpClientWrapper.Verify(x => x.SendMessage(It.IsAny<object>(), "oauth/token"), Times.Once);
            Assert.IsAssignableFrom<HmrcTokenResponse>(actual);
        }
    }
}