using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Testing;
using Fix = SFA.DAS.Configuration.UnitTests.AzureTableStorage.ConfigurationBootstrapperTestsFixture;

namespace SFA.DAS.Configuration.UnitTests.AzureTableStorage
{
    [TestFixture]
    [Parallelizable]
    public class ConfigurationBootstrapperTests : FluentTest<ConfigurationBootstrapperTestsFixture>
    {
        [Test]
        public void WhenGettingEnvironmentVariables_OnDeveloperMachineAndNoEnvironmentVariablesAreSet_ThenDefaultsAreProvided()
        {
            Test(f => f.SetEnvironmentVariables(), f => f.GetEnvironmentVariables(), (f, r) => f.AssertAreDefaults(r));
        }
        
        [TestCase("LOCAL")]
        [TestCase("AT")]
        [TestCase("TEST")]
        [TestCase("TEST2" )]
        [TestCase("PREPROD")]
        [TestCase("PROD")]
        [TestCase("MO")]
        [TestCase("DEMO")]
        public void WhenGettingEnvironmentVariables_EnvironmentVariablesAreSet_ThenValuesFromEnvironmentVariablesAreProvided(string environmentName)
        {
            Test(f => f.SetEnvironmentVariables(storageConnectionString: Fix.ExampleString, environmentName: environmentName), f => f.GetEnvironmentVariables(), (f, r) => f.AssertValues(Fix.ExampleString, environmentName));
        }

        [TestCase("AT")]
        [TestCase("TEST")]
        [TestCase("TEST2" )]
        [TestCase("PREPROD")]
        [TestCase("PROD")]
        [TestCase("MO")]
        [TestCase("DEMO")]
        public void WhenGettingEnvironmentVariables_InCloudEnvironmentAndStorageConnectionStringEnvironmentVariableIsMissing_ThenExceptionIsThrown(string environmentName)
        {
            TestException(f => f.SetEnvironmentVariables(environmentName: environmentName), f => f.GetEnvironmentVariables(), (f, r) => r.Should().Throw<Exception>());
        }
        
        [Test]
        public void WhenGettingEnvironmentVariables_EnvironmentNameEnvironmentVariableIsMissingAndStorageConnectionStringEnvironmentVariableIsPresent_ThenGivenStorageConnectionStringAndDefaultDeveloperEnvironmentNameAreProvided()
        {
            TestException(f => f.SetEnvironmentVariables(storageConnectionString: Fix.ExampleString), f => f.GetEnvironmentVariables(), (f, r) => f.AssertValues(Fix.ExampleString, "LOCAL"));
        }
    }

    public class ConfigurationBootstrapperTestsFixture
    {
        public const string ExampleString = "Xyz";
            
        public void SetEnvironmentVariables(string storageConnectionString = null, string environmentName = null)
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
            AssertValues("UseDevelopmentStorage=true", "LOCAL");
        }

        public void AssertValues(string storageConnectionString, string environmentName)
        {
            storageConnectionString.Should().Be(storageConnectionString);
            environmentName.Should().Be(environmentName);
        }
    }
}