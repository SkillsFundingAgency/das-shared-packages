using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.VacancyServices.Search.UnitTests.IntegrationTests
{
    [TestFixture]
    public class WhenReadingLocationsFromElasticsearch
    {
        [Test]
        [Category("integration")]
        public async Task GivenASearchCriteriaThatHasRepresentativeDataThenIGetDataReturned()
        {
            var criteria = "cov";
            var searchClient = new LocationSearchClient(new LocationSearchClientConfiguration()
            {
                HostName = ConfigurationManager.AppSettings.Get("elasticsearchUrl"),
                Index = ConfigurationManager.AppSettings.Get("elasticsearchIndex")
            });

            var result = searchClient.Search(criteria);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Locations);
            Assert.IsTrue(result.Locations.Any());
        }
    }
}
