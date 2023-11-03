using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Provider.Shared.UI.UnitTests.Extensions;

[Parallelizable]
public class WhenConstructionTheExternalUrlLocally
{
    private Mock<IOptions<ProviderSharedUIConfiguration>> _sharedUiConfiguration;
    private Mock<IConfiguration> _configuration;
    private ExternalUrlHelper _helper;

    [SetUp]
    public void Arrange()
    {
        var config = new ProviderSharedUIConfiguration
        {
            DashboardUrl = "https://test.local"
        };
        _sharedUiConfiguration = new Mock<IOptions<ProviderSharedUIConfiguration>>();
        _sharedUiConfiguration.Setup(x => x.Value).Returns(config);
        _configuration = new Mock<IConfiguration>();
    }

    [Test]
    public void Then_The_Url_Is_Built_Using_Localhost_And_Supplied_Port()
    {
        //Arrange
        var controller = "test-controller";
        var subDomain = "testDomain";
        var localPort = "7123";

        _configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("LOCAL");
        _configuration.Setup(x => x.GetSection("LocalPorts")[subDomain]).Returns(localPort);

        _helper = new ExternalUrlHelper(_sharedUiConfiguration.Object, _configuration.Object);

        //Act
        var actual = _helper.GenerateUrl(new UrlParameters
        {
            Controller = controller,
            SubDomain = subDomain
        });

        //Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual($"https://localhost:{localPort}/{controller}", actual);
    }

    [Test]
    public void Then_The_Url_Is_Built_Normally_If_No_Port_Supplied()
    {
        //Arrange
        var controller = "test-controller";
        var subDomain = "testDomain";

        _configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("LOCAL");
        _configuration.Setup(x => x.GetSection("LocalPorts")[subDomain]).Returns("");

        _helper = new ExternalUrlHelper(_sharedUiConfiguration.Object, _configuration.Object);

        //Act
        var actual = _helper.GenerateUrl(new UrlParameters
        {
            Controller = controller,
            SubDomain = subDomain
        });

        //Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual($"https://{subDomain}.test.local/{controller}", actual);
    }
}