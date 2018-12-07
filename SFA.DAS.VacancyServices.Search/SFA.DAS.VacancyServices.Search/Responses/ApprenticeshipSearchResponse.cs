using System;
using SFA.DAS.VacancyServices.Search.Requests;

namespace SFA.DAS.VacancyServices.Search.Responses
{
    using System.Collections.Generic;
    using Entities;

    public class ApprenticeshipSearchResponse
    {
        public ApprenticeshipSearchResponse(
            long total,
            IEnumerable<ApprenticeshipSearchResult> results, 
            IEnumerable<AggregationResult> aggregationResults,
            ApprenticeshipSearchRequestParameters searchParameters)
        {
            Total = total;
            Results = results;
            AggregationResults = aggregationResults;
            SearchParameters = searchParameters;
        }

        public long Total { get;}
        public IEnumerable<ApprenticeshipSearchResult> Results { get;}
        public IEnumerable<AggregationResult> AggregationResults { get;}
        public ApprenticeshipSearchRequestParameters SearchParameters { get;}

        public double TotalPages => Math.Ceiling((double) Total / SearchParameters.PageSize);
    }
}
