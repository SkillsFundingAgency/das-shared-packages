using SFA.DAS.VacancyServices.Search.Requests;

namespace SFA.DAS.VacancyServices.Search.Responses
{
    using System.Collections.Generic;
    using Entities;

    public class TraineeshipSearchResponse
    {
        public TraineeshipSearchResponse(
            long total,
            IEnumerable<TraineeshipSearchResult> results, 
            TraineeshipSearchRequestParameters searchParameters)
        {
            Total = total;
            Results = results;
            SearchParameters = searchParameters;
        }

        public long Total { get;}
        public IEnumerable<TraineeshipSearchResult> Results { get;}
        public TraineeshipSearchRequestParameters SearchParameters { get;}
    }
}
