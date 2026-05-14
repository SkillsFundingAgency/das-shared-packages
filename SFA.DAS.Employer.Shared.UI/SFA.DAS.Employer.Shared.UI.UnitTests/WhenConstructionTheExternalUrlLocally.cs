using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.Employer.Shared.UI.Extensions;
using SFA.DAS.Employer.Shared.UI.Models;

namespace SFA.DAS.Employer.Shared.UI.UnitTests;

public class WhenConstructionTheExternalUrlLocally
{
    private Mock<IOptions<EmployerSharedUIConfiguration>> _sharedUiConfiguration;
    private ExternalUrlHelper? _helper;

    [SetUp]
    public void Arrange()
    {
        var config = new EmployerSharedUIConfiguration
        {
            DashboardUrl = "https://test-eas.local.com",
            LocalPorts = new Dictionary<string, string>
             {
                 { "testDomain", "7123" }
             },
            ResourceEnvironmentName = "LOCAL"
        };
        _sharedUiConfiguration = new Mock<IOptions<EmployerSharedUIConfiguration>>();
        _sharedUiConfiguration.Setup(x => x.Value).Returns(config);
    }

    [Test]
    public void Then_The_Url_Is_Built_Using_Localhost_And_Supplied_Port()
    {
        //Arrange
        var controller = "test-controller";
        var subDomain = "testDomain";

        _helper = new ExternalUrlHelper(_sharedUiConfiguration.Object);

        //Act
        var actual = _helper.GenerateUrl(new UrlParameters
        {
            Controller = controller,
            SubDomain = subDomain
        });

        //Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual($"https://testDomain.test-eas.local.com/{controller}", actual);
    }

    [Test]
    public void Then_The_Url_Is_Built_Normally_If_No_Port_Supplied()
    {
        //Arrange
        var controller = "test-controller";
        var subDomain = "testDomain";

        _helper = new ExternalUrlHelper(_sharedUiConfiguration.Object);

        //Act
        var actual = _helper.GenerateUrl(new UrlParameters
        {
            Controller = controller,
            SubDomain = subDomain
        });

        //Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual($"https://{subDomain}.test-eas.local.com/{controller}", actual);
    }
}