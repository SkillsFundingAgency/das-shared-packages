using System;
using System.Collections.Generic;
using SFA.DAS.Recruit.Vacancies.Client.Entities;

namespace SFA.DAS.Recruit.Vacancies.Client
{
    public interface IClient
    {
        Vacancy GetVacancy(long vacancyReference);
        IList<Vacancy> GetLiveVacancies();
        void SubmitApplication(Application application);
        void WithdrawApplication(long vacancyReference, Guid candidateId);
    }
}