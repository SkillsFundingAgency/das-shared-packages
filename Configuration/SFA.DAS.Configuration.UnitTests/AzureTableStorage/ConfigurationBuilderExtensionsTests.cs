using System;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Testing;

namespace SFA.DAS.Configuration.UnitTests.AzureTableStorage
{
    [TestFixture, Parallelizable]
    public class ConfigurationBuilderExtensionsTests : FluentTest<ConfigurationBuilderExtensionsTestsFixture>
    {
        [Test, Ignore("WIP")]
        public void AddAzureTableStorage_WhenSetupOptionsIsInvoked_ThenOptionsContainDefaults()
        {
            Test(f => f.AddAzureTableStorageWithOptions());
        }
        
        [Test, Ignore("WIP")]
        public void AddAzureTableStorage_WhenOptionsAreSuppliedWithEnvironmentNameEnvironmentVariableNameAndNoEnvironmentName_Then()
        {
            Test(f => f.AddAzureTableStorageWithOptions());
        }
    }

    public class ConfigurationBuilderExtensionsTestsFixture
    {
        public Mock<IConfigurationBuilder> ConfigurationBuilder { get; set; }
        public Action<StorageOptions> SetupOptions { get; set; }
        
        public ConfigurationBuilderExtensionsTestsFixture()
        {
            ConfigurationBuilder = new Mock<IConfigurationBuilder>();
            SetupOptions = so => { };
        }

        public ConfigurationBuilderExtensionsTestsFixture ArrangeOptionsAreSuppliedWithEnvironmentNameEnvironmentVariableNameAndNoEnvironmentName()
        {
            return this;
        }
        
        public void AddAzureTableStorageWithOptions()
        {
            ConfigurationBuilderExtensions.AddAzureTableStorage(ConfigurationBuilder.Object, SetupOptions);
        }
    }
}