using NUnit.Framework;
using SFA.DAS.Elastic;
using System;
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

            var hostName = ConfigurationManager.AppSettings.Get("elasticsearchUrl");
            var index = ConfigurationManager.AppSettings.Get("elasticsearchIndex");
            var username = ConfigurationManager.AppSettings.Get("elasticsearchUsername");
            var password = ConfigurationManager.AppSettings.Get("elasticsearchPassword");

            var client = new ElasticClientConfiguration(new Uri(hostName), username, password)
                .CreateClientFactory();

            var searchClient = new LocationSearchClient(client, index);

            var result = searchClient.Search(criteria);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Locations);
            Assert.IsTrue(result.Locations.Any());
        }
    }
}
