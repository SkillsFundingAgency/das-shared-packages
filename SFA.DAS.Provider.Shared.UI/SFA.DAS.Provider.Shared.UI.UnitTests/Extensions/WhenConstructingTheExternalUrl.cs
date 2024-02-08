using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Provider.Shared.UI.UnitTests.Extensions
{
    [Parallelizable]
    public class WhenConstructingTheExternalUrl
    {
        private Mock<IOptions<ProviderSharedUIConfiguration>> _sharedUiConfiguration;
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
            _helper = new ExternalUrlHelper(_sharedUiConfiguration.Object, Mock.Of<IConfiguration>());
        }

        [TestCase("https://test.local")]
        [TestCase("https://test.local/")]
        public void Then_The_Url_Is_Built_From_Action_Controller_And_External_Url(string expectedBaseUrl)
        {
            //Arrange
            var action = "test-action";
            var controller = "test-controller";

            //Act
            var actual = _helper.GenerateUrl(new UrlParameters{
                Id = "123", 
                Controller = controller, 
                Action = action
            });

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual($"https://test.local/123/{controller}/{action}", actual);
        }

        [Test]
        public void Then_The_Url_Is_Built_From_Action_Controller_And_External_Url_And_IncludesId_If_Supplied()
        {
            //Arrange
            var action = "test-action";
            var controller = "test-controller";
            var id = "ABC123";

            //Act
            var actual = _helper.GenerateUrl(new UrlParameters{
                Id = id, 
                Controller = controller, 
                Action = action
            });

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual($"https://test.local/{id}/{controller}/{action}", actual);
        }

        [Test]
        public void Then_The_Url_Builds_From_Optional_Parameters()
        {
            //Arrange
            var controller = "test-controller";
            var id = "ABC123";

            //Act
            var actual = _helper.GenerateUrl(new UrlParameters{
                Id = id, 
                Controller = controller
            });

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual($"https://test.local/{id}/{controller}", actual);
        }

        [Test]
        public void Then_The_Url_Builds_From_Controller()
        {
            //Arrange
            var controller = "test-controller";

            //Act
            var actual = _helper.GenerateUrl(new UrlParameters{
                Controller = controller
            });

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual($"https://test.local/{controller}", actual);
        }

        [Test]
        public void Then_The_SubDomain_Is_Set_If_Passed()
        {
            //Arrange
            var controller = "test-controller";
            var subDomain = "testDomain";

            //Act
            var actual = _helper.GenerateUrl(new UrlParameters{
                Controller = controller, 
                SubDomain = subDomain
            });

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual($"https://{subDomain}.test.local/{controller}", actual);
        }

        [Test]
        public void Then_The_Query_String_Is_Appended()
        {
            //Arrange
            var controller = "test-controller";
            var subDomain = "testDomain";
            var queryString = "?test=12345";

            //Act
            var actual = _helper.GenerateUrl(new UrlParameters{
                Controller = controller, 
                SubDomain = subDomain,
                QueryString = queryString
            });

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual($"https://{subDomain}.test.local/{controller}?test=12345", actual);
        }

        [Test]
        public void Then_The_Url_Builds_From_Relative_Url()
        {
            //Arrange
            var relativeRoute = "test/45/route";

            //Act
            var actual = _helper.GenerateUrl(new UrlParameters
            {
                Id = "457rt",
                Controller = "34h",
                Action = "q345t",
                QueryString = "sdf98j",
                RelativeRoute = relativeRoute
                
            });

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual($"https://test.local/{relativeRoute}", actual);
        }
    }
}