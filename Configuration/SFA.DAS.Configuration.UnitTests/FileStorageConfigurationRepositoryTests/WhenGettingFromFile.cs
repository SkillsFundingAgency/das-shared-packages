using NUnit.Framework;
using SFA.DAS.Configuration.FileStorage;

namespace SFA.DAS.Configuration.UnitTests.FileStorageConfigurationRepositoryTests
{
    public class WhenGettingFromFile
    {
        private FileStorageConfigurationRepository _configurationRepository;

        [SetUp]
        public void Arrange()
        {
            _configurationRepository = new FileStorageConfigurationRepository();
        }

        [Test]
        public void ThenTheFileIsCorrectlyReadfromTheAppDataFolder()
        {
            //Act
            var config = _configurationRepository.Get("test", "test", "1.0");

            //Assert
            Assert.IsNotNull(config);
        }
    }
}
