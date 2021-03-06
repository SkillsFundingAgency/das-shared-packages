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
    public class WhenICallTheHmrcServiceForEmprefWithPrivilegedAccess
    {
        private const string ExpectedAuthToken = "789654321AGFVD";
        private readonly string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
        private readonly string ExpectedClientId = "654321";
        private readonly string ExpectedClientSecret = "my_secret";
        private readonly string ExpectedName = "My Company Name";
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

            _apprenticeshipLevyApiClient = new Mock<IApprenticeshipLevyApiClient>();

            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _apprenticeshipLevyApiClient.Setup(x => x.GetEmployerDetails(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new EmpRefLevyInformation
                {
                    Employer = new Employer { Name = new Name { EmprefAssociatedName = ExpectedName } },
                    Links = new Links()
                });

            _tokenService = new Mock<ITokenServiceApiClient>();
            _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAuthToken });


            _hmrcService = new HmrcService(_configuration, _httpClientWrapper.Object,
                _apprenticeshipLevyApiClient.Object, _tokenService.Object, new NoopExecutionPolicy(), null, null, new Mock<ILog>().Object);
        }

        [Test]
        public async Task ThenTheLevInformationIsReturned()
        {
            //Arrange
            var empRef = "123/AB12345";

            //Act
            var actual = await _hmrcService.GetEmprefInformation(empRef);

            //Assert
            Assert.IsAssignableFrom<EmpRefLevyInformation>(actual);
            Assert.AreEqual(ExpectedName, actual.Employer.Name.EmprefAssociatedName);
        }
    }
}