using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SFA.DAS.Configuration.UnitTests.ConfigurationServiceTests
{
    public class WhenGettingAsync
    {
        private Mock<IConfigurationRepository> _configurationRepo;
        private ConfigurationOptions _options;
        private ConfigurationService _configurationService;

        [SetUp]
        public void Arrange()
        {
            var devConfig = JsonConvert.SerializeObject(TestConfiguration.GetDefault());
            var sitConfig = JsonConvert.SerializeObject(new TestConfiguration { Text = "SIT Details" });
            _configurationRepo = new Mock<IConfigurationRepository>();
            _configurationRepo.Setup(repo => repo.GetAsync("SFA.DAS.Configuration.UnitTests", "Dev", "1.0")).Returns(Task.FromResult(devConfig));
            _configurationRepo.Setup(repo => repo.GetAsync("SFA.DAS.Configuration.UnitTests", "SIT", "1.0")).Returns(Task.FromResult(sitConfig));

            _options = new ConfigurationOptions("SFA.DAS.Configuration.UnitTests", "Dev", "1.0");

            _configurationService = new ConfigurationService(_configurationRepo.Object, _options);
        }

        [Test]
        public async Task ThenItShouldReturnNullWhenConfigurationNotFound()
        {
            // Arrange
            var options = new ConfigurationOptions(_options.ServiceName, "PROD", _options.VersionNumber);
            var configurationService = new ConfigurationService(_configurationRepo.Object, options);

            // Act
            var actual = await configurationService.GetAsync<TestConfiguration>();

            // Assert
            Assert.Null(actual);
        }

        [Test]
        public async Task ThenItShouldReturnAnInstanceOfTestConfigurationWhenConfigurationFound()
        {
            // Act
            var actual = await _configurationService.GetAsync<TestConfiguration>();

            // Assert
            Assert.NotNull(actual);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectNumberValue()
        {
            // Act
            var actual = await _configurationService.GetAsync<TestConfiguration>();

            // Assert
            Assert.AreEqual(1234, actual.Number);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectAmountValue()
        {
            // Act
            var actual = await _configurationService.GetAsync<TestConfiguration>();

            // Assert
            Assert.AreEqual(12.34, actual.Amount);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectToggleValue()
        {
            // Act
            var actual = await _configurationService.GetAsync<TestConfiguration>();

            // Assert
            Assert.AreEqual(true, actual.Toggle);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectTextValue()
        {
            // Act
            var actual = await _configurationService.GetAsync<TestConfiguration>();

            // Assert
            Assert.AreEqual("Some details", actual.Text);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectCollectionValue()
        {
            // Act
            var actual = await _configurationService.GetAsync<TestConfiguration>();

            // Assert
            Assert.AreEqual(3, actual.Collection.Count);
            Assert.AreEqual("a", actual.Collection[0]);
            Assert.AreEqual("b", actual.Collection[1]);
            Assert.AreEqual("c", actual.Collection[2]);
        }

        [Test]
        public async Task ThenItShouldReturnConfigForEnvironment()
        {
            // Arrange
            var options = new ConfigurationOptions(_options.ServiceName, "SIT", _options.VersionNumber);
            var configurationService = new ConfigurationService(_configurationRepo.Object, options);

            // Act
            var actual = await configurationService.GetAsync<TestConfiguration>();
            
            // Assert
            Assert.AreEqual("SIT Details", actual.Text);
        }
    }
}
