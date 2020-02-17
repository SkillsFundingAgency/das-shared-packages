using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.VacancyServices.Search.UnitTests.IntegrationTests
{
    [TestFixture]
    public class LocationSearchClientIntegrationTests
    {
        [Test]
        [Category("integration")]
        public async Task GetLocations()
        {
            var criteria = "cov";
            var searchClient = new LocationSearchClient(new LocationSearchClientConfiguration()
            {
                HostName = "http://localhost:9200/",
                Index = "local-faa-locations"
            });

            var result = searchClient.Search(criteria);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Locations);
            Assert.IsTrue(result.Locations.Any());
        }
    }
}
