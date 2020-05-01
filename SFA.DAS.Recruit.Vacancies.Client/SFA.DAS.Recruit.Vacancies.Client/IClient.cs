using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Recruit.Vacancies.Client.Entities;

namespace SFA.DAS.Recruit.Vacancies.Client
{
    public interface IClient
    {
        Vacancy GetPublishedVacancy(long vacancyReference);
        IList<Vacancy> GetLiveVacancies();
        Task<List<Vacancy>> GetLiveVacanciesAsync(int pageSize, int pageNumber);
        void SubmitApplication(Application application);
        void WithdrawApplication(long vacancyReference, Guid candidateId);
        void DeleteCandidate(Guid candidateId);
        Vacancy GetLiveVacancy(long vacancyReference);
    }
}