using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SFA.DAS.Employer.Shared.UI.Configuration;
using SFA.DAS.EmployerUrlHelper;
using Xunit;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests
{
    public class UrlBuilderTests
    {
        [Fact]
        public void AccountsLinkWithNullAccountId_ReturnRouteLink()
        {
            var options = Substitute.For<IOptionsMonitor<MaPageConfiguration>>();
            
            var config = new MaPageConfiguration {
                Routes = new MaRoutes {
                    Accounts = new System.Collections.Generic.Dictionary<string, string>{{"Routename", "ProperLink/{0}"}}
                }
            };

            options.CurrentValue.Returns(config);

            var linkGenerator = Substitute.For<ILinkGenerator>();

            linkGenerator.AccountsLink(Arg.Any<string>()).Returns("ProperLink");
            linkGenerator.AccountsLink("").Returns("RouteLink");            

            var urlBuilder = new UrlBuilder(Substitute.For<ILogger<UrlBuilder>>(), 
                options, linkGenerator);
            
            Assert.Equal("RouteLink", urlBuilder.AccountsLink()); 
            Assert.Equal("RouteLink", urlBuilder.AccountsLink("Routename", "")); 
            Assert.Equal("RouteLink", urlBuilder.AccountsLink("Routename")); 
            Assert.Equal("RouteLink", urlBuilder.AccountsLink("Routename", null)); 

            Assert.Equal("ProperLink", urlBuilder.AccountsLink("Routename", "ABC123")); 
        }
    }
}