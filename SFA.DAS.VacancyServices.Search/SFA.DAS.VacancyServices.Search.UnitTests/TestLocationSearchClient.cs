namespace SFA.DAS.VacancyServices.Search.UnitTests
{
    using System.Collections.Generic;
    using Entities;

    public class TestLocationSearchClient : LocationSearchClient
    {
        public TestLocationSearchClient() : base(null, null) { }

        public override IEnumerable<LocationSearchResult> SearchExact(string placeName, int maxResults = MaxResults)
        {
            return new[] { new LocationSearchResult { Name = "search-exact", Latitude = 1, Longitude = 1 } };
        }

        public override IEnumerable<LocationSearchResult> SearchPrefixed(string placeName, int maxResults = MaxResults)
        {
            return new[] { new LocationSearchResult { Name = "search-prefixed", Latitude = 2, Longitude = 2 } };
        }

        public override IEnumerable<LocationSearchResult> SearchFuzzy(string placeName, int maxResults = MaxResults)
        {
            return new[] { new LocationSearchResult { Name = "search-fuzzy", Latitude = 3, Longitude = 3 } };
        }
    }

}
