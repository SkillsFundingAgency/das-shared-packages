using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace SFA.DAS.Configuration.UnitTests.ConfigurationServiceTests
{
    public class WhenGetting
    {
        private Mock<IConfigurationRepository> _configurationRepo;
        private ConfigurationOptions _options;
        private ConfigurationService _configurationService;
        private string _devConfig;

        [SetUp]
        public void Arrange()
        {
            _devConfig = JsonConvert.SerializeObject(TestConfiguration.GetDefault());
            var sitConfig = JsonConvert.SerializeObject(new TestConfiguration { Text = "SIT Details" });
            _configurationRepo = new Mock<IConfigurationRepository>();
            _configurationRepo.Setup(repo => repo.Get("SFA.DAS.Configuration.UnitTests", "Dev", "1.0")).Returns(_devConfig);
            _configurationRepo.Setup(repo => repo.Get("SFA.DAS.Configuration.UnitTests", "SIT", "1.0")).Returns(sitConfig);

            _options = new ConfigurationOptions("SFA.DAS.Configuration.UnitTests", "Dev", "1.0");

            _configurationService = new ConfigurationService(_configurationRepo.Object, _options);
        }

        [Test]
        public void ThenItShouldReturnNullWhenConfigurationNotFound()
        {
            // Arrange
            var options = new ConfigurationOptions(_options.ServiceName, "PROD", _options.VersionNumber);
            var configurationService = new ConfigurationService(_configurationRepo.Object, options);

            // Act
            var actual = configurationService.Get<TestConfiguration>();

            // Assert
            Assert.Null(actual);
        }

        [Test]
        public void ThenItShouldReturnAnInstanceOfTestConfigurationWhenConfigurationFound()
        {
            // Act
            var actual = _configurationService.Get<TestConfiguration>();

            // Assert
            Assert.NotNull(actual);
        }

        [Test]
        public void ThenItShouldReturnCorrectNumberValue()
        {
            // Act
            var actual = _configurationService.Get<TestConfiguration>();

            // Assert
            Assert.AreEqual(1234, actual.Number);
        }

        [Test]
        public void ThenItShouldReturnCorrectAmountValue()
        {
            // Act
            var actual = _configurationService.Get<TestConfiguration>();

            // Assert
            Assert.AreEqual(12.34, actual.Amount);
        }

        [Test]
        public void ThenItShouldReturnCorrectToggleValue()
        {
            // Act
            var actual = _configurationService.Get<TestConfiguration>();

            // Assert
            Assert.AreEqual(true, actual.Toggle);
        }

        [Test]
        public void ThenItShouldReturnCorrectTextValue()
        {
            // Act
            var actual = _configurationService.Get<TestConfiguration>();

            // Assert
            Assert.AreEqual("Some details", actual.Text);
        }

        [Test]
        public void ThenItShouldReturnCorrectCollectionValue()
        {
            // Act
            var actual = _configurationService.Get<TestConfiguration>();

            // Assert
            Assert.AreEqual(3, actual.Collection.Count);
            Assert.AreEqual("a", actual.Collection[0]);
            Assert.AreEqual("b", actual.Collection[1]);
            Assert.AreEqual("c", actual.Collection[2]);
        }

        [Test]
        public void ThenItShouldReturnConfigForEnvironment()
        {
            // Arrange
            var options = new ConfigurationOptions(_options.ServiceName, "SIT", _options.VersionNumber);
            var configurationService = new ConfigurationService(_configurationRepo.Object, options);

            // Act
            var actual = configurationService.Get<TestConfiguration>();
            
            // Assert
            Assert.AreEqual("SIT Details", actual.Text);
        }

        [Test]
        public void ThenIShouldGetConfigurationFromCacheIfItExists()
        {
            // Arrange
            var cache = new Mock<IConfigurationCache>();
            var options = new ConfigurationOptions(_options.ServiceName, "SIT", _options.VersionNumber);
            var configurationService = new ConfigurationService(_configurationRepo.Object, options, cache.Object);

            cache.Setup(x => x.Get<string>(It.IsAny<string>())).Returns(_devConfig);
            _configurationRepo.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            // Act
            configurationService.Get<TestConfiguration>();

            // Assert
            cache.Verify(x => x.Get<string>(It.IsAny<string>()), Times.Once);
            _configurationRepo.Verify(x => x.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenIShouldGetConfigurationFromCacheIfItExistsAsAsync()
        {
            // Arrange
            var cache = new Mock<IConfigurationCache>();
            var options = new ConfigurationOptions(_options.ServiceName, "SIT", _options.VersionNumber);
            var configurationService = new ConfigurationService(_configurationRepo.Object, options, cache.Object);

            cache.Setup(x => x.Get<string>(It.IsAny<string>())).Returns(_devConfig);
            _configurationRepo.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            // Act
            await configurationService.GetAsync<TestConfiguration>();

            // Assert
            cache.Verify(x => x.Get<string>(It.IsAny<string>()), Times.Once);
            _configurationRepo.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ThenIShouldGetConfigurationFromRepoIfCacheIsOutdated()
        {
            // Arrange
            var cache = new Mock<IConfigurationCache>();
            var options = new ConfigurationOptions(_options.ServiceName, "SIT", _options.VersionNumber);
            var configurationService = new ConfigurationService(_configurationRepo.Object, options, cache.Object);

            cache.Setup(x => x.Get<string>(It.IsAny<string>()));
            _configurationRepo.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            // Act
            configurationService.Get<TestConfiguration>();

            // Assert
            cache.Verify(x => x.Get<string>(It.IsAny<string>()), Times.Once);
            _configurationRepo.Verify(x => x.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ThenIShouldGetConfigurationFromRepoIfCacheIsOutdatedAsAsync()
        {
            // Arrange
            var cache = new Mock<IConfigurationCache>();
            var options = new ConfigurationOptions(_options.ServiceName, "SIT", _options.VersionNumber);
            var configurationService = new ConfigurationService(_configurationRepo.Object, options, cache.Object);

            cache.Setup(x => x.Get<string>(It.IsAny<string>()));
            _configurationRepo.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("");

            // Act
            await configurationService.GetAsync<TestConfiguration>();

            // Assert
            cache.Verify(x => x.Get<string>(It.IsAny<string>()), Times.Once);
            _configurationRepo.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ThenTheCacheShouldBeUpdatedIfItIsOutOfDate()
        {
            // Arrange
            var cache = new Mock<IConfigurationCache>();
            var options = new ConfigurationOptions(_options.ServiceName, "SIT", _options.VersionNumber);
            var configurationService = new ConfigurationService(_configurationRepo.Object, options, cache.Object);

            cache.Setup(x => x.Get<string>(It.IsAny<string>()));
            _configurationRepo.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(_devConfig);

            // Act
            configurationService.Get<TestConfiguration>();

            // Assert
            cache.Verify(x => x.Get<string>(It.IsAny<string>()), Times.Once);
            cache.Verify(x => x.Set(typeof(TestConfiguration).FullName, It.IsAny<string>()), Times.Once);

        }

        [Test]
        public async Task ThenTheCacheShouldBeUpdatedIfItIsOutOfDateAsAsync()
        {
            // Arrange
            var cache = new Mock<IConfigurationCache>();
            var options = new ConfigurationOptions(_options.ServiceName, "SIT", _options.VersionNumber);
            var configurationService = new ConfigurationService(_configurationRepo.Object, options, cache.Object);

            cache.Setup(x => x.Get<string>(It.IsAny<string>()));
            _configurationRepo.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_devConfig);

            // Act
            await configurationService.GetAsync<TestConfiguration>();

            // Assert
            cache.Verify(x => x.Get<string>(It.IsAny<string>()), Times.Once);
            cache.Verify(x => x.Set(typeof(TestConfiguration).FullName, It.IsAny<string>()), Times.Once);
        }
    }
}
