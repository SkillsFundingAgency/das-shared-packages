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

        [Test]
        public void AddAzureTableStorage_WhenOptionsAreSuppliedWithEnvironmentNameEnvironmentVariableNameAndNoEnvironmentName_ThenAddCalledWithConfigurationSourceWithCorrectEnvironmentName()
        {
            Test(f => f.ArrangeOptionsAreSuppliedWithEnvironmentNameEnvironmentVariableNameAndNoEnvironmentName(),
                f => f.AddAzureTableStorageWithOptions(),
                f => f.VerifyAddCalledWithConfigurationSourceWithCorrectEnvironmentName());
        }
        
        [Test]
        public void AddAzureTableStorage_WhenOptionsAreSuppliedWithStorageConnectionStringEnvironmentVariableNameAndNoStorageConnectionString_ThenAddCalledWithConfigurationSourceWithCorrectConnectionString()
        {
            Test(f => f.ArrangeOptionsAreSuppliedWithStorageConnectionStringEnvironmentVariableNameAndNoStorageConnectionString(),
                f => f.AddAzureTableStorageWithOptions(),
                f => f.VerifyAddCalledWithConfigurationSourceWithCorrectConnectionString());
        }
    }

    public class ConfigurationBuilderExtensionsTestsFixture
    {
        public Mock<IConfigurationBuilder> ConfigurationBuilder { get; set; }
        public Action<StorageOptions> SetupOptions { get; set; }
        public string[] ConfigurationKeys { get; set; }
        public string ExpectedEnvironmentName { get; set; }
        public const string EnvironmentNameFromVariable = nameof(EnvironmentNameFromVariable);
        public string ExpectedConnectionString { get; set; }
        public const string ConnectionStringFromVariable = nameof(ConnectionStringFromVariable);
        
        public ConfigurationBuilderExtensionsTestsFixture()
        {
            ConfigurationBuilder = new Mock<IConfigurationBuilder>();
            ConfigurationKeys = new[] {"Key"};
            SetupOptions = so => so.ConfigurationKeys = ConfigurationKeys;

            // clear defaults, which are likely to be set on the developer's machine
            Environment.SetEnvironmentVariable("APPSETTING_EnvironmentName", EnvironmentNameFromVariable);
            Environment.SetEnvironmentVariable("APPSETTING_ConfigurationStorageConnectionString", ConnectionStringFromVariable);
        }

        public ConfigurationBuilderExtensionsTestsFixture ArrangeOptionsAreSuppliedWithEnvironmentNameEnvironmentVariableNameAndNoEnvironmentName()
        {
            const string optionSuppliedEnvironmentNameEnvironmentVariableName = nameof(optionSuppliedEnvironmentNameEnvironmentVariableName);
            
            // this arrangement is actually the default, but we explicitly set it in case the defaults change
            SetupOptions = so =>
            {
                so.ConfigurationKeys = ConfigurationKeys;
                so.EnvironmentNameEnvironmentVariableName = optionSuppliedEnvironmentNameEnvironmentVariableName;
                so.EnvironmentName = null;
            };

            // not here, just test
            ExpectedEnvironmentName = "EnvironmentNameFromOptionSuppliedVariableName";
            Environment.SetEnvironmentVariable(optionSuppliedEnvironmentNameEnvironmentVariableName, ExpectedEnvironmentName);
            
            return this;
        }

        public ConfigurationBuilderExtensionsTestsFixture ArrangeOptionsAreSuppliedWithStorageConnectionStringEnvironmentVariableNameAndNoStorageConnectionString()
        {
            const string optionSuppliedStorageConnectionStringEnvironmentVariableName
                = nameof(optionSuppliedStorageConnectionStringEnvironmentVariableName);
            
            // this arrangement is actually the default, but we explicitly set it in case the defaults change
            SetupOptions = so =>
            {
                so.ConfigurationKeys = ConfigurationKeys;
                so.StorageConnectionStringEnvironmentVariableName = optionSuppliedStorageConnectionStringEnvironmentVariableName;
                so.StorageConnectionString = null;
            };

            // not here, just test
            ExpectedConnectionString = "ConnectionStringFromOptionSuppliedVariableName";
            Environment.SetEnvironmentVariable(optionSuppliedStorageConnectionStringEnvironmentVariableName, ExpectedConnectionString);
            
            return this;
        }

        public void AddAzureTableStorageWithOptions()
        {
            ConfigurationBuilderExtensions.AddAzureTableStorage(ConfigurationBuilder.Object, SetupOptions);
        }

        public void VerifyAddCalledWithConfigurationSourceWithCorrectEnvironmentName()
        {
            ConfigurationBuilder.Verify(cb => cb.Add(It.Is<AzureTableStorageConfigurationSource>( 
                s => s.EnvironmentName == ExpectedEnvironmentName)));
        }
        
        public void VerifyAddCalledWithConfigurationSourceWithCorrectConnectionString()
        {
            ConfigurationBuilder.Verify(cb => cb.Add(It.Is<AzureTableStorageConfigurationSource>( 
                s => s.ConnectionString == ExpectedConnectionString)));
        }
    }
}