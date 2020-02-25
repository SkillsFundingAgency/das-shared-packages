using NUnit.Framework;
using SFA.DAS.Elastic;
using System.Configuration;
using System.Linq;

namespace SFA.DAS.VacancyServices.Search.UnitTests.IntegrationTests
{
    [TestFixture]
    public class WhenReadingLocationsFromElasticsearch
    {
        [Test]
        [Category("integration")]
        public void GivenASearchCriteriaThatHasRepresentativeDataThenIGetDataReturned()
        {
            var criteria = "cov";

            var config = new LocationSearchClientConfiguration()
            {
                HostName = ConfigurationManager.AppSettings.Get("elasticsearchUrl"),
                Index = ConfigurationManager.AppSettings.Get("elasticsearchIndex"),
                Username = ConfigurationManager.AppSettings.Get("elasticsearchUsername"),
                Password = ConfigurationManager.AppSettings.Get("elasticsearchPassword"),
            };

            var client = new ElasticClientConfiguration()
                .UseSingleNodeConnectionPool(config.HostName, config.Username, config.Password)
                .CreateClientFactory()
                .CreateClient();

            var searchClient = new LocationSearchClient(client, config);

            var result = searchClient.Search(criteria);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Locations);
            Assert.IsTrue(result.Locations.Any());
        }
    }
}
