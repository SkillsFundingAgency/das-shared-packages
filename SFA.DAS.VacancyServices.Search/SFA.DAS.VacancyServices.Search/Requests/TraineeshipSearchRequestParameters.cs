namespace SFA.DAS.VacancyServices.Search.Requests
{
    public class TraineeshipSearchRequestParameters
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int SearchRadius { get; set; }
        public VacancySearchSortType SortType { get; set; }
        public string VacancyReference { get; set; }

        public bool IsLatLongSearch => Latitude.HasValue && Longitude.HasValue;
    }
}
