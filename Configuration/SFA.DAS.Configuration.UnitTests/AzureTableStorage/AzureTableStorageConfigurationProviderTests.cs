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
    [TestFixture, Parallelizable]
    public class AzureTableStorageConfigurationProviderTests : FluentTest<AzureTableStorageConfigurationProviderTestsFixture>
    {
        [Test, TestCaseSource(typeof(AzureTableStorageConfigurationProviderTestsSource), nameof(AzureTableStorageConfigurationProviderTestsSource.TestCasesWithPrefix))]
        public void WhenReadingTables_ThenConfigDataShouldBeCorrect(IEnumerable<(string configKey, string json)> sourceConfigs, IEnumerable<(string key, string value)> expected)
        {
            Test(f => f.SetConfigs(sourceConfigs), f => f.Load(), f => f.AssertData(expected));
        }

        [Test, TestCaseSource(typeof(AzureTableStorageConfigurationProviderTestsSource), nameof(AzureTableStorageConfigurationProviderTestsSource.TestCasesWithoutPrefix))]
        public void WhenReadingTables_AndConfigSetToNotPrefixKeys_ThenConfigKeyShouldNotBePrefixed(IEnumerable<(string configKey, string json)> sourceConfigs, IEnumerable<(string key, string value)> expected)
        {
            Test(f => f.SetConfigs(sourceConfigs, false), f => f.Load(), f => f.AssertData(expected));
        }

        [Test]
        public void WhenReadingTablesAndConfigRowIsNotFound_ThenExceptionIsThrown()
        {
            TestException(f => f.ArrangeConfigNotFound(), f => f.Load(), (f, r) => r.Should().Throw<Exception>().WithMessage("Configuration row not found*"));
        }
    }

    public class AzureTableStorageConfigurationProviderTestsSource
    {
        private static IEnumerable<(string configKey, string json)> GenerateSource(int tableCount)
        {
            return Enumerable.Range(0, tableCount).Select(cnt => ($"t{cnt}", $@"{{""k{cnt}"": ""v{cnt}""}}"));
        }

        public static IEnumerable TestCasesWithPrefix
        {
            get
            {
                yield return new TestCaseData(new[] {("t1", @"{}")}, new (string, string)[] {}).SetName("EmptyJsonFromSingleTableWithPrefix");
                yield return new TestCaseData(new[] {("t1", @"{""k1"": ""v1""}")}, new[] {("t1:k1", "v1")}).SetName("SingleItemInFlatJsonFromSingleTableWithPrefix");
                yield return new TestCaseData(new[] {("t1", @"{""k1"": ""v1"", ""k2"": ""v2""}")}, new[] {("t1:k1", "v1"), ("t1:k2", "v2")}).SetName("MultipleItemsInFlatJsonFromSingleTableWithPrefix");
                yield return new TestCaseData(new[] {("t1", @"{""k1"": ""v1""}"), ("t2", @"{""k2"": ""v2""}")}, new[] {("t1:k1", "v1"), ("t2:k2", "v2")}).SetName("FlatJsonsFromMultipleTablesWithPrefix");
                yield return new TestCaseData(new[] {("t1", @"{""ss1"": {""k1"": ""v1""}}")}, new[] {("t1:ss1:k1", "v1")}).SetName("NestedJsonFromSingleTableWithPrefix");
                yield return new TestCaseData(new[] {("t1", @"{""ss1"": {""sss1"": {""k1"": ""v1""}}}")}, new[] {("t1:ss1:sss1:k1", "v1")}).SetName("MultiLevelNestedJsonFromSingleTableWithPrefix");
                yield return new TestCaseData(new[] {("t1", @"{""ss1"": {""k1"": ""v1""}, ""k2"": ""v2""}")}, new[] {("t1:ss1:k1", "v1"), ("t1:k2", "v2")}).SetName("OneSubSectionAndOneItemFromSingleTableWithPrefix");
                yield return new TestCaseData(new[] {("t1", @"{""ss1"": {""k1"": ""v1""}, ""ss2"": {""k2"": ""v2""}}")}, new[] {("t1:ss1:k1", "v1"), ("t1:ss2:k2", "v2")}).SetName("MultipleSubSectionsFromSingleTableWithPrefix");
                yield return new TestCaseData(new[] {("t1", @"{""ss1"": {""k1"": ""v1""}, ""ss2"": {""k2"": ""v2""}}"), ("t2", @"{""ss3"": {""k3"": ""v3""}, ""ss4"": {""k4"": ""v4""}}")}, new[] {("t1:ss1:k1", "v1"), ("t1:ss2:k2", "v2"), ("t2:ss3:k3", "v3"), ("t2:ss4:k4", "v4")}).SetName("MultipleSubSectionsFromMultipleTablesWithPrefix");
                yield return new TestCaseData(GenerateSource(10), new[] {("t0:k0", "v0"), ("t1:k1", "v1"), ("t2:k2", "v2"), ("t3:k3", "v3"), ("t4:k4", "v4"), ("t5:k5", "v5"), ("t6:k6", "v6"), ("t7:k7", "v7"), ("t8:k8", "v8"), ("t9:k9", "v9")}).SetName("ManyTablesWithPrefix");
                yield return new TestCaseData(new[] { ("SFA.DAS.LongLongLongLongLong.ConfigurationKeyV3", @"{""KeyKeyKeyKey"": ""value!value$value^value*value8value<value""}") }, new[] { ("SFA.DAS.LongLongLongLongLong.ConfigurationKeyV3:KeyKeyKeyKey", "value!value$value^value*value8value<value") }).SetName("SingleTableWithRealisticNamesWithPrefix");
            }
        }

        public static IEnumerable TestCasesWithoutPrefix
        {
            get
            {
                yield return new TestCaseData(new[] { ("t1", @"{}") }, new (string, string)[] { }).SetName("EmptyJsonFromSingleTableNoPrefix");
                yield return new TestCaseData(new[] { ("t1", @"{""k1"": ""v1""}") }, new[] { ("k1", "v1") }).SetName("SingleItemInFlatJsonFromSingleTableNoPrefix");
                yield return new TestCaseData(new[] { ("t1", @"{""k1"": ""v1"", ""k2"": ""v2""}") }, new[] { ("k1", "v1"), ("k2", "v2") }).SetName("MultipleItemsInFlatJsonFromSingleTableNoPrefix");
                yield return new TestCaseData(new[] { ("t1", @"{""k1"": ""v1""}"), ("t2", @"{""k2"": ""v2""}") }, new[] { ("k1", "v1"), ("k2", "v2") }).SetName("FlatJsonsFromMultipleTablesNoPrefix");
                yield return new TestCaseData(new[] { ("t1", @"{""ss1"": {""k1"": ""v1""}}") }, new[] { ("ss1:k1", "v1") }).SetName("NestedJsonFromSingleTableNoPrefix");
                yield return new TestCaseData(new[] { ("t1", @"{""ss1"": {""sss1"": {""k1"": ""v1""}}}") }, new[] { ("ss1:sss1:k1", "v1") }).SetName("MultiLevelNestedJsonFromSingleTableNoPrefix");
                yield return new TestCaseData(new[] { ("t1", @"{""ss1"": {""k1"": ""v1""}, ""k2"": ""v2""}") }, new[] { ("ss1:k1", "v1"), ("k2", "v2") }).SetName("OneSubSectionAndOneItemFromSingleTableNoPrefix");
                yield return new TestCaseData(new[] { ("t1", @"{""ss1"": {""k1"": ""v1""}, ""ss2"": {""k2"": ""v2""}}") }, new[] { ("ss1:k1", "v1"), ("ss2:k2", "v2") }).SetName("MultipleSubSectionsFromSingleTableNoPrefix");
                yield return new TestCaseData(new[] { ("t1", @"{""ss1"": {""k1"": ""v1""}, ""ss2"": {""k2"": ""v2""}}"), ("t2", @"{""ss3"": {""k3"": ""v3""}, ""ss4"": {""k4"": ""v4""}}") }, new[] { ("ss1:k1", "v1"), ("ss2:k2", "v2"), ("ss3:k3", "v3"), ("ss4:k4", "v4") }).SetName("MultipleSubSectionsFromMultipleTablesNoPrefix");
                yield return new TestCaseData(GenerateSource(10), new[] { ("k0", "v0"), ("k1", "v1"), ("k2", "v2"), ("k3", "v3"), ("k4", "v4"), ("k5", "v5"), ("k6", "v6"), ("k7", "v7"), ("k8", "v8"), ("k9", "v9") }).SetName("ManyTablesNoPrefix");
                yield return new TestCaseData(new[] { ("SFA.DAS.LongLongLongLongLong.ConfigurationKeyV3", @"{""KeyKeyKeyKey"": ""value!value$value^value*value8value<value""}") }, new[] { ("KeyKeyKeyKey", "value!value$value^value*value8value<value") }).SetName("SingleTableWithRealisticNamesNoPrefix");
                yield return new TestCaseData(new[] { ("t1", @"{""ss1"": {""k1"": ""v1""}, ""ss2"": {""k2"": ""v2""}}"), ("t2", @"{""ss1"": {""k1"": ""v3""}, ""ss2"": {""k2"": ""v4""}}") }, new[] { ("ss1:k1", "v3"), ("ss2:k2", "v4") }).SetName("MultipleTableWithSameKeys");

            }
        }
    }
    
    public class TestableAzureTableStorageConfigurationProvider : AzureTableStorageConfigurationProvider
    {
        public TestableAzureTableStorageConfigurationProvider(CloudStorageAccount cloudStorageAccount, string environmentName, IEnumerable<string> configurationKeys, bool prefixConfigurationKeys = true)
            : base(cloudStorageAccount, environmentName, configurationKeys, prefixConfigurationKeys)
        {
        } 
        
        protected override TableOperation GetTableRowOperation(string configurationKey)
        {
            var tableEntity = new Mock<IConfigurationRow>();
            tableEntity.SetupGet(te => te.RowKey).Returns(configurationKey);

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
        
        public void SetConfigs(IEnumerable<(string configKey, string json)> configs, bool prefixConfigurationKeySetting = true)
        {
            ConfigProvider = new TestableAzureTableStorageConfigurationProvider(CloudStorageAccount.Object, EnvironmentName, configs.Select(c => c.configKey), prefixConfigurationKeySetting);

            foreach (var config in configs)
            {
                var configurationRow = new AzureTableStorageConfigurationProvider.ConfigurationRow {Data = config.json};

                CloudTable.Setup(ct => ct.ExecuteAsync(It.Is<TableOperation>(to => to.Entity.RowKey == config.configKey)))
                    .ReturnsAsync(new TableResult { Result = configurationRow });
            }
        }

        public void ArrangeConfigNotFound()
        {
            const string configKey = "ConfigRowNotInTable";
            ConfigProvider = new TestableAzureTableStorageConfigurationProvider(CloudStorageAccount.Object, EnvironmentName, new[] {configKey});

            CloudTable.Setup(ct => ct.ExecuteAsync(It.Is<TableOperation>(to => to.Entity.RowKey == configKey)))
                .ReturnsAsync(new TableResult { HttpStatusCode = 404 });
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