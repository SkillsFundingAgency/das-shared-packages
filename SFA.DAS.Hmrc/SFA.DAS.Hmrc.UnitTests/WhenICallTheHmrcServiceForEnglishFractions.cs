using System;
using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Client;
using HMRC.ESFA.Levy.Api.Types;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.ActiveDirectory;
using SFA.DAS.Hmrc.Configuration;
using SFA.DAS.Hmrc.Http;
using SFA.DAS.Hmrc.Models;
using SFA.DAS.NLog.Logger;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.TokenService.Api.Types;

namespace SFA.DAS.Hmrc.UnitTests
{
    internal class WhenICallTheHmrcServiceForEnglishFractions
    {
        private const string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
        private const string ExpectedClientId = "654321";
        private const string ExpectedScope = "emp_ref";
        private const string ExpectedClientSecret = "my_secret";
        private const string ExpectedTotpToken = "789654321AGFVD";
        private const string ExpectedAuthToken = "GFRT567";
        private const string ExpectedOgdClientId = "123AOK564";
        private const string EmpRef = "111/ABC";
        private Mock<IApprenticeshipLevyApiClient> _apprenticeshipLevyApiClient;
        private Mock<IAzureAdAuthenticationService> _azureAdAuthService;
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
                ClientSecret = ExpectedClientSecret,
                OgdClientId = ExpectedOgdClientId,
                AzureAppKey = "123TRG",
                AzureClientId = "TYG567",
                AzureResourceId = "Resource1",
                AzureTenant = "test",
                UseHiDataFeed = false
            };

            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _httpClientWrapper.Setup(x => x.SendMessage("", $"oauth/token?client_secret={ExpectedTotpToken}&client_id={ExpectedOgdClientId}&grant_type=client_credentials&scopes=read:apprenticeship-levy")).ReturnsAsync(JsonConvert.SerializeObject(new HmrcTokenResponse { AccessToken = ExpectedAuthToken }));

            _tokenService = new Mock<ITokenServiceApiClient>();
            _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAuthToken });

            _apprenticeshipLevyApiClient = new Mock<IApprenticeshipLevyApiClient>();

            _azureAdAuthService = new Mock<IAzureAdAuthenticationService>();
            _azureAdAuthService.Setup(x =>
                    x.GetAuthenticationResult(_configuration.AzureClientId, _configuration.AzureAppKey,
                        _configuration.AzureResourceId, _configuration.AzureTenant))
                .ReturnsAsync(ExpectedAuthToken);

            _hmrcService = new HmrcService(_configuration, _httpClientWrapper.Object, _apprenticeshipLevyApiClient.Object, _tokenService.Object, new NoopExecutionPolicy(), null, _azureAdAuthService.Object, new Mock<ILog>().Object);
        }

        [Test]
        public async Task ThenIShouldGetBackDeclarationsForAGivenEmpRef()
        {
            //Arrange

            var englishFractions = new EnglishFractionDeclarations();
            _apprenticeshipLevyApiClient.Setup(x => x.GetEmployerFractionCalculations(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(englishFractions);

            //Act
            var result = await _hmrcService.GetEnglishFractions(EmpRef);

            //Assert
            _apprenticeshipLevyApiClient.Verify(
                x => x.GetEmployerFractionCalculations(ExpectedAuthToken, It.IsAny<string>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()), Times.Once);
            Assert.AreEqual(englishFractions, result);
        }

        [Test]
        public async Task ThenTheFractionsAreFilterByTheDateRangeWhenPassed()
        {
            //Arrange
            var expectedDate = new DateTime(2017, 04, 29);

            //Act
            await _hmrcService.GetEnglishFractions(EmpRef, expectedDate);
            _apprenticeshipLevyApiClient.Verify(x => x.GetEmployerFractionCalculations(ExpectedAuthToken,
                It.IsAny<string>(), It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>()), Times.Once);
        }


        [Test]
        public async Task ThenIfTheConfigurationIsSetToUseTheMiDataThenTheAzureAuthServiceIsCalled()
        {
            //Arrange
            _configuration.UseHiDataFeed = true;


            //Act
            await _hmrcService.GetLevyDeclarations(EmpRef);

            //Assert
            _tokenService.Verify(x => x.GetPrivilegedAccessTokenAsync(), Times.Never);
            _azureAdAuthService.Verify(x => x.GetAuthenticationResult(_configuration.ClientId, _configuration.AzureAppKey, _configuration.AzureResourceId, _configuration.AzureTenant), Times.Once);
        }
    }
}