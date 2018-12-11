﻿using System.Collections.Generic;
using SFA.DAS.VacancyServices.Search.Entities;

namespace SFA.DAS.VacancyServices.Search.Requests
{
    public class ApprenticeshipSearchRequestParameters
    {
        public string ApprenticeshipLevel { get; set; }
        public string CategoryCode { get; set; }
        public bool DisabilityConfidentOnly { get; set; }
        public IEnumerable<int> ExcludeVacancyIds { get; set; }
        public string Keywords { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public ApprenticeshipSearchField SearchField { get; set; }
        public int SearchRadius { get; set; }
        public VacancySearchSortType SortType { get; set; }
        public string[] SubCategoryCodes { get; set; }
        public VacancyLocationType VacancyLocationType { get; set; }
        public string VacancyReference { get; set; }

        public bool IsLatLongSearch => Latitude.HasValue && Longitude.HasValue;
    }
}