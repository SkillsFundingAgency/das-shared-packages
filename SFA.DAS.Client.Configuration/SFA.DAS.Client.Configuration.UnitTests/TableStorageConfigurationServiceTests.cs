using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.Client.Configuration.UnitTests
{
    [TestFixture]
    [Parallelizable]
    public class TableStorageConfigurationServiceTests : FluentTest<TableStorageConfigurationServiceTestsFixture>
    {
        [Test]
        public void Get_ShouldReturnCorrectData()
        {
            Run(f => f.SetupEnvironmentService().SetupAzureTableStorageConnectionAdapter(), f => f.GetResult(), (f, r) => r.SampleProperty.Should().Be(f.ExpectedPropertyValue));
        }

        [Test]
        public void Get_ShouldCallTheEnvironmentServiceCorrectlyForTheEnvironment()
        {
            Run(f => f.SetupEnvironmentService().SetupAzureTableStorageConnectionAdapter(), f => f.GetResult(), f => f.VerifyEnvironmentServiceWasCalledCorrectlyForTheEnvironment());
        }

        [Test]
        public void Get_ShouldCallTheEnvironmentServiceCorrectlyForTheConfigurationStorageConnectionString()
        {
            Run(f => f.SetupEnvironmentService().SetupAzureTableStorageConnectionAdapter(), f => f.GetResult(), f => f.VerifyEnvironmentServiceWasCalledCorrectlyForTheConfigurationStorageConnectionString());
        }

        [Test]
        public void Get_ShouldCallTheAzureTableStorageConnectionAdapterCorrectlyForTheTableReference()
        {
            Run(f => f.SetupEnvironmentService().SetupAzureTableStorageConnectionAdapter(), f => f.GetResult(), f => f.VerifyAzureTableStorageConnectionAdapterWasCalledCorrectlyForTheTableReference());
        }

        [Test]
        public void Get_ShouldCallTheAzureTableStorageConnectionAdapterCorrectlyForTheRetrieveOperation()
        {
            Run(f => f.SetupEnvironmentService().SetupAzureTableStorageConnectionAdapter(), f => f.GetResult(), f => f.VerifyAzureTableStorageConnectionAdapterWasCalledCorrectlyForTheRetrieveOperation());
        }

        [Test]
        public void Get_ShouldCallTheAzureTableStorageConnectionAdapterCorrectlyForExecute()
        {
            Run(f => f.SetupEnvironmentService().SetupAzureTableStorageConnectionAdapter(), f => f.GetResult(), f => f.VerifyAzureTableStorageConnectionAdapterWasCalledCorrectlyForExecute());
        }

        [Test]
        public void Get_ShouldCallTheAzureTableStorageConnectionAdapterCorrectlyForTheRetrieveOperationWithDefaultedEnvironmentName()
        {
            Run(f => f.SetupEnvironmentServiceWithoutEnvironmentName().SetupAzureTableStorageConnectionAdapter(), f => f.GetResult(), f => f.VerifyAzureTableStorageConnectionAdapterWasCalledCorrectlyForTheRetrieveOperationWithDefaultedEnvironmentName());
        }

        [Test]
        public void Get_ShouldCallTheAzureTableStorageConnectionAdapterCorrectlyForTheRetrieveOperationWithDefaultedConnectionString()
        {
            Run(f => f.SetupEnvironmentServiceWithoutConnectionString().SetupAzureTableStorageConnectionAdapter(), f => f.GetResult(), f => f.VerifyAzureTableStorageConnectionAdapterWasCalledCorrectlyForTheTableReferenceWithDefaultedConnectionString());
        }
    }

    public class TableStorageConfigurationServiceTestsFixture
    {
        public TableStorageConfigurationService TableStorageConfigurationService { get; set; }
        public Mock<IEnvironmentService> MockEnvironmentService { get; set; }
        public Mock<IAzureTableStorageConnectionAdapter> MockAzureTableStorageConnectionAdapter { get; set; }
        public string ExpectedPropertyValue { get; set; }
        public string ExpectedEnvironment { get; set; }
        public string ExpectedConfigurationConnectionString { get; set; }
        public CloudTable ExpectedCloudTable { get; set; }
        public TableOperation ExpectedOperation { get; set; }

        public const string DefaultEnvironment = "LOCAL";
        public const string DefaultConnectionString = "UseDevelopmentStorage=true";


        public TableStorageConfigurationServiceTestsFixture()
        {
            MockEnvironmentService = new Mock<IEnvironmentService>();

            MockAzureTableStorageConnectionAdapter = new Mock<IAzureTableStorageConnectionAdapter>();

            TableStorageConfigurationService = new TableStorageConfigurationService(MockEnvironmentService.Object, MockAzureTableStorageConnectionAdapter.Object);

            ExpectedPropertyValue = "Sample Property Value";
            ExpectedEnvironment = "Test Environment";
            ExpectedConfigurationConnectionString = "Test Connection String";
            ExpectedCloudTable = new CloudTable(new Uri("http://example.com"));
        }

        public SampleDataType GetResult()
        {
            return TableStorageConfigurationService.Get<SampleDataType>();
        }

        public TableStorageConfigurationServiceTestsFixture SetupEnvironmentService()
        {
            MockEnvironmentService.Setup(x => x.GetVariable(EnvironmentVariableNames.Environment)).Returns(ExpectedEnvironment);
            MockEnvironmentService.Setup(x => x.GetVariable(EnvironmentVariableNames.ConfigurationStorageConnectionString)).Returns(ExpectedConfigurationConnectionString);

            return this;
        }

        public TableStorageConfigurationServiceTestsFixture SetupEnvironmentServiceWithoutEnvironmentName()
        {
            MockEnvironmentService.Setup(x => x.GetVariable(EnvironmentVariableNames.ConfigurationStorageConnectionString)).Returns(ExpectedConfigurationConnectionString);

            return this;
        }

        public TableStorageConfigurationServiceTestsFixture SetupEnvironmentServiceWithoutConnectionString()
        {
            MockEnvironmentService.Setup(x => x.GetVariable(EnvironmentVariableNames.Environment)).Returns(ExpectedEnvironment);

            return this;
        }

        public void VerifyEnvironmentServiceWasCalledCorrectlyForTheEnvironment()
        {
            MockEnvironmentService.Verify(x => x.GetVariable(EnvironmentVariableNames.Environment));
        }

        public void VerifyEnvironmentServiceWasCalledCorrectlyForTheConfigurationStorageConnectionString()
        {
            MockEnvironmentService.Verify(x => x.GetVariable(EnvironmentVariableNames.ConfigurationStorageConnectionString));
        }

        public TableStorageConfigurationServiceTestsFixture SetupAzureTableStorageConnectionAdapter()
        {
            MockAzureTableStorageConnectionAdapter.Setup(x => x.GetTableReference(It.IsAny<string>(), It.IsAny<string>())).Returns(ExpectedCloudTable);
            MockAzureTableStorageConnectionAdapter.Setup(x => x.GetRetrieveOperation(ExpectedEnvironment, $"{Assembly.GetAssembly(typeof(SampleDataType)).GetName().Name}_1.0")).Returns(ExpectedOperation);
            MockAzureTableStorageConnectionAdapter.Setup(x => x.Execute(ExpectedCloudTable, ExpectedOperation))
                .Returns(new TableResult() {Result = new DynamicTableEntity { Properties = new Dictionary<string, EntityProperty>( new[]{ new KeyValuePair<string, EntityProperty>("Data", new EntityProperty(GetJsonData()) ) } )}});

            return this;
        }

        public void VerifyAzureTableStorageConnectionAdapterWasCalledCorrectlyForTheTableReference()
        {
            MockAzureTableStorageConnectionAdapter.Verify(x => x.GetTableReference(ExpectedConfigurationConnectionString, "Configuration"));
        }

        public void VerifyAzureTableStorageConnectionAdapterWasCalledCorrectlyForTheRetrieveOperation()
        {
            MockAzureTableStorageConnectionAdapter.Verify(x => x.GetRetrieveOperation(ExpectedEnvironment, $"{Assembly.GetAssembly(typeof(SampleDataType)).GetName().Name}_1.0"));
        }

        public void VerifyAzureTableStorageConnectionAdapterWasCalledCorrectlyForTheRetrieveOperationWithDefaultedEnvironmentName()
        {
            MockAzureTableStorageConnectionAdapter.Verify(x => x.GetRetrieveOperation(DefaultEnvironment, $"{Assembly.GetAssembly(typeof(SampleDataType)).GetName().Name}_1.0"));
        }

        public void VerifyAzureTableStorageConnectionAdapterWasCalledCorrectlyForTheTableReferenceWithDefaultedConnectionString()
        {
            MockAzureTableStorageConnectionAdapter.Verify(x => x.GetTableReference(DefaultConnectionString, "Configuration"));
        }

        public void VerifyAzureTableStorageConnectionAdapterWasCalledCorrectlyForExecute()
        {
            MockAzureTableStorageConnectionAdapter.Verify(x => x.Execute(ExpectedCloudTable, ExpectedOperation));
        }

        private string GetJsonData()
        {
            return JsonConvert.SerializeObject(new SampleDataType { SampleProperty = ExpectedPropertyValue });
        }
    }

    public class SampleDataType
    {
        public string SampleProperty { get; set; }
    }
}
