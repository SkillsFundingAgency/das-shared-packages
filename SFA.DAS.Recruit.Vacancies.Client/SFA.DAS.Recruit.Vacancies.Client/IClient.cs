using System;
using SFA.DAS.Recruit.Vacancies.Client.Entities;
using System.Collections.Generic;

namespace SFA.DAS.Recruit.Vacancies.Client
{
    public interface IClient
    {
        LiveVacancy GetVacancy(long vacancyReference);
        IList<LiveVacancy> GetLiveVacancies();
        void SubmitApplication(Application application);
        void WithdrawApplication(long vacancyReference, Guid candidateId);
    }
}