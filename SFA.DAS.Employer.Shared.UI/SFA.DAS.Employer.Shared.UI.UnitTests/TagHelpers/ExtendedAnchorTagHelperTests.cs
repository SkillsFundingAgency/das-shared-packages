using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using SFA.DAS.Employer.Shared.UI.Extensions;
using SFA.DAS.Employer.Shared.UI.Models;
using SFA.DAS.Employer.Shared.UI.TagHelpers;

namespace SFA.DAS.Employer.Shared.UI.UnitTests.TagHelpers;

public class ExtendedAnchorTagHelperTests
{
    private Mock<IExternalUrlHelper> _externalUrlHelper;
    private Mock<IHtmlGenerator> _htmlGenerator;

    [SetUp]
    public void SetUp()
    {
        _externalUrlHelper = new Mock<IExternalUrlHelper>();
        _htmlGenerator = new Mock<IHtmlGenerator>();
    }

    [Test]
    public void Then_The_Href_Is_Set_From_The_Helper()
    {
        //Arrange
        var expectedUrl = "https://test.local/test-controller/test-action";
        _externalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedUrl);

        _htmlGenerator.Setup(x => x.GenerateActionLink(
            It.IsAny<Microsoft.AspNetCore.Mvc.Rendering.ViewContext>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<object>(),
            It.IsAny<object>())).Returns(new TagBuilder("a"));

        var tagHelper = new ExtendedAnchorTagHelper(_htmlGenerator.Object, _externalUrlHelper.Object)
        {
            ExternalAction = "test-action",
            ExternalController = "test-controller",
            ExternalId = "123",
            ExternalSubDomain = "testDomain",
            ExternalFolder = "test-folder",
            QueryString = "test=value",
            RelativeRoute = "test-route"
        };
        var context = new TagHelperContext(
            tagName: "a",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "a",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) =>
            {
                return Task.FromResult<TagHelperContent>(new DefaultTagHelperContent());
            });
        //Act
        tagHelper.Process(context, output);
        //Assert

        expectedUrl.Should().NotBeNull();
        expectedUrl.Should().Be((string)output.Attributes["href"].Value);
    }

    [Test]
    public void Process_CallsGenerateUrl_WithCorrectParameters()
    {
        //Arrange
        UrlParameters parameters = null;

        _htmlGenerator.Setup(x => x.GenerateActionLink(
          It.IsAny<Microsoft.AspNetCore.Mvc.Rendering.ViewContext>(),
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<object>(),
          It.IsAny<object>())).Returns(new TagBuilder("a"));

        var expectedUrl = "https://test.local/test-controller/test-action";
        _externalUrlHelper.Setup(x => x.GenerateUrl(It.IsAny<UrlParameters>())).Callback<UrlParameters>(p => parameters = p)
        .Returns(expectedUrl);

        var tagHelper = new ExtendedAnchorTagHelper(_htmlGenerator.Object, _externalUrlHelper.Object)
        {
            ExternalAction = "test-action",
            ExternalController = "test-controller",
            ExternalId = "123",
            ExternalSubDomain = "testDomain",
            ExternalFolder = "test-folder",
            QueryString = "test=value",
            RelativeRoute = "test-route"
        };

        var context = new TagHelperContext(
            tagName: "a",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput("a", new TagHelperAttributeList(), (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        tagHelper.Process(context, output);

        parameters.Should().NotBeNull();
        parameters.Action.Should().Be("test-action");
        parameters.Controller.Should().Be("test-controller");
        parameters.Id.Should().Be("123");
        parameters.SubDomain.Should().Be("testDomain");
        parameters.Folder.Should().Be("test-folder");
        parameters.QueryString.Should().Be("test=value");
        parameters.RelativeRoute.Should().Be("test-route");
    }
}