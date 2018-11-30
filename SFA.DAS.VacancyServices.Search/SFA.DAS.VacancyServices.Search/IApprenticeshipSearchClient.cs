using SFA.DAS.VacancyServices.Search.Requests;

namespace SFA.DAS.VacancyServices.Search
{
    using Responses;
    using System.Collections.Generic;

    public interface IApprenticeshipSearchClient
    {
        ApprenticeshipSearchResponse Search(ApprenticeshipSearchRequestParameters searchParameters);
        IEnumerable<int> GetAllVacancyIds();
    }
}