using SFA.DAS.VacancyServices.Search.Requests;

namespace SFA.DAS.VacancyServices.Search
{
    using System.Collections.Generic;
    using Responses;

    public interface ITraineeshipSearchClient
    {
        TraineeshipSearchResponse Search(TraineeshipSearchRequestParameters searchParameters);
        IEnumerable<int> GetAllVacancyIds();
    }
}