using System;
using System.Collections.Generic;
using SFA.DAS.VacancyServices.Search.Entities;

namespace SFA.DAS.VacancyServices.Search.Requests
{
    public class ApprenticeshipSearchRequestParameters
    {
        public string ApprenticeshipLevel { get; set; }
        public bool CalculateSubCategoryAggregations { get; set; }
        public string CategoryCode { get; set; }
        public bool DisabilityConfidentOnly { get; set; }
        public IEnumerable<int> ExcludeVacancyIds { get; set; }
        public List<string> FrameworkLarsCodes { get; set; } = new List<string>();
        public List<string> StandardLarsCodes { get; set; } = new List<string>();
        public DateTime? FromDate { get; set; }
        public string Keywords { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public ApprenticeshipSearchField SearchField { get; set; }
        public double? SearchRadius { get; set; }
        public VacancySearchSortType SortType { get; set; }
        public string[] SubCategoryCodes { get; set; }
        public VacancyLocationType VacancyLocationType { get; set; }
        public string VacancyReference { get; set; }

        public bool CanFilterByGeoDistance => Latitude.HasValue && Longitude.HasValue && SearchRadius.HasValue;

        public bool CanSortByGeoDistance => Latitude.HasValue && Longitude.HasValue;
    }
}
