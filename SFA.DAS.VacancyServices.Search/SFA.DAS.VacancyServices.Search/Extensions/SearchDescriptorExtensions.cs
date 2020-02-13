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
                searchDescriptor.Sort(s => s.GeoDistance(g => g
                .Field(f => f.Location)
                .DistanceType(GeoDistanceType.Arc)
                .Unit(DistanceUnit.Miles)
                .Mode(SortMode.Min)
                .Points(new GeoLocation(searchParameters.Latitude.Value, searchParameters.Longitude.Value))));
            }

            return searchDescriptor;
        }

        internal static SearchDescriptor<TraineeshipSearchResult> TrySortByGeoDistance(this SearchDescriptor<TraineeshipSearchResult> searchDescriptor, TraineeshipSearchRequestParameters searchParameters)
        {
            if (searchParameters.CanSortByGeoDistance)
            {
                searchDescriptor.Sort(s => s.GeoDistance(g => g
                .Field(f => f.Location)
                .DistanceType(GeoDistanceType.Arc)
                .Unit(DistanceUnit.Miles)
                .Mode(SortMode.Min)
                .Points(new GeoLocation(searchParameters.Latitude.Value, searchParameters.Longitude.Value))));
            }

            return searchDescriptor;
        }
    }
}
