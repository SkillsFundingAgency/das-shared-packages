using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.GovUK.Auth.UnitTests.Services;

public class StubAuthenticationServiceTests
{
    [Test, MoqAutoData]
    public void Then_Cookies_Are_Added_To_The_Response_When_Not_Prod(
        StubAuthUserDetails model,
        [Frozen] Mock<IResponseCookies> responseCookies,
        [Frozen] Mock<IConfiguration> configuration)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
        var service = new StubAuthenticationService(configuration.Object);
        
        service.AddStubEmployerAuth(responseCookies.Object, model);
        
        responseCookies.Verify(x=>x.Append(GovUkConstants.StubAuthCookieName,JsonConvert.SerializeObject(model), It.Is<CookieOptions>(c=>c.Domain!.Equals(".test-eas.apprenticeships.education.gov.uk"))));
    }
    
    [Test, MoqAutoData]
    public void Then_Cookies_Are_Added_To_The_Response_When_local(
        StubAuthUserDetails model,
        [Frozen] Mock<IResponseCookies> responseCookies,
        [Frozen] Mock<IConfiguration> configuration)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("local");
        var service = new StubAuthenticationService(configuration.Object);
        
        service.AddStubEmployerAuth(responseCookies.Object, model);
        
        responseCookies.Verify(x=>x.Append(GovUkConstants.StubAuthCookieName,JsonConvert.SerializeObject(model), It.Is<CookieOptions>(c=>c.Domain!.Equals("localhost") && !c.IsEssential)));
    }
    
    [Test, MoqAutoData]
    public void Then_Cookies_Are_Added_To_The_Response_With_Optional_Parameters(
        StubAuthUserDetails model,
        [Frozen] Mock<IResponseCookies> responseCookies,
        [Frozen] Mock<IConfiguration> configuration)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("local");
        var service = new StubAuthenticationService(configuration.Object);
        
        service.AddStubEmployerAuth(responseCookies.Object, model, true);
        
        responseCookies.Verify(x=>x.Append(GovUkConstants.StubAuthCookieName,JsonConvert.SerializeObject(model), 
            It.Is<CookieOptions>(c=>
                c.Domain!.Equals("localhost") 
                && c.IsEssential
                && c.Secure
                && c.HttpOnly
                && c.Path!.Equals("/")
                )));
    }
    
    [Test, MoqAutoData]
    public void Then_Cookies_Are_Not_Added_To_The_Response_When_Prod(
        StubAuthUserDetails model,
        [Frozen] Mock<IResponseCookies> responseCookies,
        [Frozen] Mock<IConfiguration> configuration)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("prd");
        var service = new StubAuthenticationService(configuration.Object);
        
        service.AddStubEmployerAuth(responseCookies.Object, model);
        
        responseCookies.Verify(x=>x.Append(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Never());
    }
}