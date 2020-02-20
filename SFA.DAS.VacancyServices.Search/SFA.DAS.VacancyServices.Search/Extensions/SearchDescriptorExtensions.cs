using SFA.DAS.VacancyServices.Search.Requests;

namespace SFA.DAS.VacancyServices.Search
{
    using Entities;
    using Nest;

    internal static class SearchDescriptorExtensions
    {
        internal static SearchDescriptor<ApprenticeshipSearchResult> TrySortByGeoDistance(this SearchDescriptor<ApprenticeshipSearchResult> searchDescriptor, ApprenticeshipSearchRequestParameters searchParameters)
        {
            if (searchParameters.CanSortByGeoDistance)
            {
                searchDescriptor.SortGeoDistance(g =>
                {
                    g.PinTo(searchParameters.Latitude.Value, searchParameters.Longitude.Value)
                        .Unit(GeoUnit.Miles).OnField(f => f.Location);
                    return g;
                });
            }

            return searchDescriptor;
        }

        internal static SearchDescriptor<TraineeshipSearchResult> TrySortByGeoDistance(this SearchDescriptor<TraineeshipSearchResult> searchDescriptor, TraineeshipSearchRequestParameters searchParameters)
        {
            if (searchParameters.CanSortByGeoDistance)
            {
                searchDescriptor.SortGeoDistance(g =>
                {
                    g.PinTo(searchParameters.Latitude.Value, searchParameters.Longitude.Value)
                        .Unit(GeoUnit.Miles).OnField(f => f.Location);
                    return g;
                });
            }

            return searchDescriptor;
        }
    }
}
