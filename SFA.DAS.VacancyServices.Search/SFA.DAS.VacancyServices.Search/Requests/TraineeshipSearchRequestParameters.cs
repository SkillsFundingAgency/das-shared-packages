namespace SFA.DAS.VacancyServices.Search.Requests
{
    public class TraineeshipSearchRequestParameters
    {
        public bool DisabilityConfidentOnly { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public double? SearchRadius { get; set; }
        public VacancySearchSortType SortType { get; set; }
        public string VacancyReference { get; set; }

        public bool CanFilterByGeoDistance => Latitude.HasValue && Longitude.HasValue && SearchRadius.HasValue;

        public bool CanSortByGeoDistance => Latitude.HasValue && Longitude.HasValue;
    }
}
