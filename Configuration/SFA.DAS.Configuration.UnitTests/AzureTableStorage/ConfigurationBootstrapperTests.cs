using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Testing;

namespace SFA.DAS.Configuration.UnitTests.AzureTableStorage
{
    [TestFixture]
    [Parallelizable]
    public class ConfigurationBootstrapperTests : FluentTest<ConfigurationBootstrapperTestsFixture>
    {
        [Test]
        public void WhenGettingEnvironmentVariablesOnDeveloperMachineAndNoEnvironmentVariablesAreSet_ThenDefaultsAreProvided()
        {
            Test(f => f.SetEnvironmentVariables(), f => f.GetEnvironmentVariables(), (f, r) => f.AssertAreDefaults(r));
        }
    }

    public class ConfigurationBootstrapperTestsFixture
    {
        public void SetEnvironmentVariables(string environmentName = null, string storageConnectionString = null)
        {
            Environment.SetEnvironmentVariable("APPSETTING_EnvironmentName", environmentName, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("APPSETTING_ConfigurationStorageConnectionString", storageConnectionString, EnvironmentVariableTarget.Process);
        }

        public (string StorageConnectionString, string EnvironmentName) GetEnvironmentVariables()
        {
            return ConfigurationBootstrapper.GetEnvironmentVariables();
        }

        public void AssertAreDefaults((string StorageConnectionString, string EnvironmentName) retrievedValues)
        {
            retrievedValues.StorageConnectionString.Should().Be("UseDevelopmentStorage=true");
            retrievedValues.EnvironmentName.Should().Be("LOCAL");
        }
    }
}