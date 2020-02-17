using SFA.DAS.VacancyServices.Search.Requests;

namespace SFA.DAS.VacancyServices.Search
{
    using Entities;
    using Nest;

    internal static class SearchDescriptorExtensions
    {
        internal static SortDescriptor<ApprenticeshipSearchResult> TrySortByGeoDistance(this SortDescriptor<ApprenticeshipSearchResult> sortDescriptor, ApprenticeshipSearchRequestParameters searchParameters)
        {
            if (searchParameters.CanSortByGeoDistance)
            {
                sortDescriptor.GeoDistance(g => g
                .Field(f => f.Location)
                .DistanceType(GeoDistanceType.Arc)
                .Unit(DistanceUnit.Miles)
                .Points(new GeoLocation(searchParameters.Latitude.Value, searchParameters.Longitude.Value)));
            }

            return sortDescriptor;
        }

        internal static SortDescriptor<TraineeshipSearchResult> TrySortByGeoDistance(this SortDescriptor<TraineeshipSearchResult> sortDescriptor, TraineeshipSearchRequestParameters searchParameters)
        {
            if (searchParameters.CanSortByGeoDistance)
            {
                sortDescriptor.GeoDistance(g => g
                .Field(f => f.Location)
                .DistanceType(GeoDistanceType.Arc)
                .Unit(DistanceUnit.Miles)
                .Points(new GeoLocation(searchParameters.Latitude.Value, searchParameters.Longitude.Value)));
            }

            return sortDescriptor;
        }
    }
}
