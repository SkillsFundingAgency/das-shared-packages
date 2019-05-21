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

        [TestCase("LOCAL")]
        [TestCase("AT")]
        [TestCase("TEST")]
        [TestCase("TEST2" )]
        [TestCase("PREPROD")]
        [TestCase("PROD")]
        [TestCase("MO")]
        [TestCase("DEMO")]
        public void WhenGettingEnvironmentVariablesByCustomNames_EnvironmentVariablesAreSet_ThenValuesFromEnvironmentVariablesAreProvided(string environmentName)
        {
            Test(f => f.SetEnvironmentVariables(storageConnectionString: (Fix.CustomStorageKey, Fix.CustomStorageValue), environmentName: (Fix.CustomEnvironmentKey,Fix.CustomEnvironmentValue)), f => f.GetEnvironmentVariables(Fix.CustomStorageKey, Fix.CustomEnvironmentKey), (f, r) => f.AssertValues(r.TableStorageConnectionString, r.EnvironmentName));
        }
    }

    public class ConfigurationBootstrapperTestsFixture
    {
        public const string ExampleString = "Xyz";
        public const string CustomStorageKey = "CustomStorageKey";
        public const string CustomStorageValue = "CustomStorageKey";
        public const string CustomEnvironmentKey = "CustomEnvironmentKey";
        public const string CustomEnvironmentValue = "CustomEnvironmentValue";
        private string _expectedEnvironmentValue = "LOCAL";
        private string _expectedStorageConnectionValue = "UseDevelopmentStorage=true";

        public void SetEnvironmentVariables((string storageConnectionStringKey, string storageConnectionStringValue) storageConnectionString, (string environmentNameKey, string environmentNameValue) environmentName)
        {
            Environment.SetEnvironmentVariable(environmentName.environmentNameKey, environmentName.environmentNameValue, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable(storageConnectionString.storageConnectionStringKey, storageConnectionString.storageConnectionStringValue, EnvironmentVariableTarget.Process);
            _expectedEnvironmentValue = environmentName.environmentNameValue;
            _expectedStorageConnectionValue = storageConnectionString.storageConnectionStringValue;
        }    

        public void SetEnvironmentVariables(string storageConnectionString = null, string environmentName = null)
        {
            Environment.SetEnvironmentVariable("APPSETTING_EnvironmentName", environmentName, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("APPSETTING_ConfigurationStorageConnectionString", storageConnectionString, EnvironmentVariableTarget.Process);
        
            if (environmentName != null)
                _expectedEnvironmentValue = environmentName;
            
            if (storageConnectionString != null)
                _expectedStorageConnectionValue = storageConnectionString;
        }

        public EnvironmentVariables GetEnvironmentVariables()
        {
            return ConfigurationBootstrapper.GetEnvironmentVariables();
        }

        public EnvironmentVariables GetEnvironmentVariables(string connectionStringKey, string environmentKey)
        {
            return ConfigurationBootstrapper.GetEnvironmentVariables(connectionStringKey, environmentKey);
        }

        public void AssertAreDefaults(EnvironmentVariables retrievedValues)
        {
            AssertValues("UseDevelopmentStorage=true", "LOCAL");
        }

        public void AssertValues(string storageConnectionString, string environmentName)
        {
            storageConnectionString.Should().Be(_expectedStorageConnectionValue);
            environmentName.Should().Be(_expectedEnvironmentValue);
        }
    }
}