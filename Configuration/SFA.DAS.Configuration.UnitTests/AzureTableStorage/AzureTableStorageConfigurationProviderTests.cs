using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Testing;

namespace SFA.DAS.Configuration.UnitTests.AzureTableStorage
{
    [TestFixture]
    [Parallelizable]
    public class AzureTableStorageConfigurationProviderTests : FluentTest<AzureTableStorageConfigurationProviderTestsFixture>
    {
        [Test, TestCaseSource(typeof(AzureTableStorageConfigurationProviderTestsSource), nameof(AzureTableStorageConfigurationProviderTestsSource.TestCases))]
        public void WhenReadingTables_ThenConfigDataShouldBeCorrect(IEnumerable<(string configKey, string json)> sourceConfigs, IEnumerable<(string key, string value)> expected)
        {
            Test(f => f.SetConfigs(sourceConfigs), f => f.Load(), f => f.AssertData(expected));
        }
    }

    public class AzureTableStorageConfigurationProviderTestsSource
    {
        private static IEnumerable<(string configKey, string json)> GenerateSource(int tableCount)
        {
            return Enumerable.Range(0, tableCount).Select(cnt => ($"t{cnt}", $@"{{""k{cnt}"": ""v{cnt}""}}"));
        }

        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new[] {("t1", @"{}")}, new (string, string)[] {}).SetName("EmptyJsonFromSingleTable");
                yield return new TestCaseData(new[] {("t1", @"{""k1"": ""v1""}")}, new[] {("t1:k1", "v1")}).SetName("SingleItemInFlatJsonFromSingleTable");
                yield return new TestCaseData(new[] {("t1", @"{""k1"": ""v1"", ""k2"": ""v2""}")}, new[] {("t1:k1", "v1"), ("t1:k2", "v2")}).SetName("MultipleItemsInFlatJsonFromSingleTable");
                yield return new TestCaseData(new[] {("t1", @"{""k1"": ""v1""}"), ("t2", @"{""k2"": ""v2""}")}, new[] {("t1:k1", "v1"), ("t2:k2", "v2")}).SetName("FlatJsonsFromMultipleTables");
                yield return new TestCaseData(new[] {("t1", @"{""ss1"": {""k1"": ""v1""}}")}, new[] {("t1:ss1:k1", "v1")}).SetName("NestedJsonFromSingleTable");
                yield return new TestCaseData(new[] {("t1", @"{""ss1"": {""sss1"": {""k1"": ""v1""}}}")}, new[] {("t1:ss1:sss1:k1", "v1")}).SetName("MultiLevelNestedJsonFromSingleTable");
                yield return new TestCaseData(new[] {("t1", @"{""ss1"": {""k1"": ""v1""}, ""k2"": ""v2""}")}, new[] {("t1:ss1:k1", "v1"), ("t1:k2", "v2")}).SetName("OneSubSectionAndOneItemFromSingleTable");
                yield return new TestCaseData(new[] {("t1", @"{""ss1"": {""k1"": ""v1""}, ""ss2"": {""k2"": ""v2""}}")}, new[] {("t1:ss1:k1", "v1"), ("t1:ss2:k2", "v2")}).SetName("MultipleSubSectionsFromSingleTable");
                yield return new TestCaseData(new[] {("t1", @"{""ss1"": {""k1"": ""v1""}, ""ss2"": {""k2"": ""v2""}}"), ("t2", @"{""ss3"": {""k3"": ""v3""}, ""ss4"": {""k4"": ""v4""}}")}, new[] {("t1:ss1:k1", "v1"), ("t1:ss2:k2", "v2"), ("t2:ss3:k3", "v3"), ("t2:ss4:k4", "v4")}).SetName("MultipleSubSectionsFromMultipleTables");
                yield return new TestCaseData(GenerateSource(10), new[] {("t0:k0", "v0"), ("t1:k1", "v1"), ("t2:k2", "v2"), ("t3:k3", "v3"), ("t4:k4", "v4"), ("t5:k5", "v5"), ("t6:k6", "v6"), ("t7:k7", "v7"), ("t8:k8", "v8"), ("t9:k9", "v9")}).SetName("ManyTables");
                yield return new TestCaseData(new[] {("SFA.DAS.LongLongLongLongLong.ConfigurationKeyV3", @"{""KeyKeyKeyKey"": ""value!value$value^value*value8value<value""}")}, new[] {("SFA.DAS.LongLongLongLongLong.ConfigurationKeyV3:KeyKeyKeyKey", "value!value$value^value*value8value<value")}).SetName("SingleTableWithRealistNames");
            }
        }  
    }
    
    public class TestableAzureTableStorageConfigurationProvider : AzureTableStorageConfigurationProvider
    {
        public TestableAzureTableStorageConfigurationProvider(CloudStorageAccount cloudStorageAccount, string environment, IEnumerable<string> configNames)
            : base(cloudStorageAccount, environment, configNames)
        {
        }
        
        protected override TableOperation GetOperation(string serviceName)
        {
            var tableEntity = new Mock<IConfigurationRow>();
            tableEntity.SetupGet(te => te.RowKey).Returns(serviceName);

            var tableOperation = TableOperation.Retrieve<ConfigurationRow>("", "");
            typeof(TableOperation).GetProperty("Entity").SetValue(tableOperation, tableEntity.Object);
            return tableOperation;
        }
        
        public IDictionary<string, string> PublicData => Data;
    }
    
    public class AzureTableStorageConfigurationProviderTestsFixture
    {
        public const string EnvironmentName = "PROD";
        public const string ConfigurationTableName = "Configuration";
        public TestableAzureTableStorageConfigurationProvider ConfigProvider { get; set; }
        public Mock<CloudStorageAccount> CloudStorageAccount { get; set; }
        public Mock<CloudTableClient> CloudTableClient { get; set; }
        public Mock<CloudTable> CloudTable { get; set; }

        public AzureTableStorageConfigurationProviderTestsFixture()
        {
            var dummyUri = new Uri("http://example.com/");
            var dummyStorageCredentials = new StorageCredentials();
            CloudTable = new Mock<CloudTable>(dummyUri);
            CloudTableClient = new Mock<CloudTableClient>(dummyUri, dummyStorageCredentials);
            CloudTableClient.Setup(ctc => ctc.GetTableReference(ConfigurationTableName)).Returns(CloudTable.Object);
            CloudStorageAccount = new Mock<CloudStorageAccount>(dummyStorageCredentials, dummyUri, dummyUri, dummyUri, dummyUri);
            CloudStorageAccount.Setup(csa => csa.CreateCloudTableClient()).Returns(CloudTableClient.Object);
        }
        
        public void SetConfigs(IEnumerable<(string configKey, string json)> configs)
        {
            ConfigProvider = new TestableAzureTableStorageConfigurationProvider(CloudStorageAccount.Object, EnvironmentName, configs.Select(c => c.configKey));

            foreach (var config in configs)
            {
                var configurationRow = new AzureTableStorageConfigurationProvider.ConfigurationRow {Data = config.json};

                CloudTable.Setup(ct => ct.ExecuteAsync(It.Is<TableOperation>(to => to.Entity.RowKey == config.configKey)))
                    .ReturnsAsync(new TableResult { Result = configurationRow });
            }
        }

        public void Load()
        {
            ConfigProvider.Load();
        }

        public void AssertData(IEnumerable<(string key, string value)> expectedData)
        {
            var expectedDictionary = expectedData.ToDictionary(ed => ed.key, ed => ed.value);
            ConfigProvider.PublicData.Should().Equal(expectedDictionary);
        }
    }
}