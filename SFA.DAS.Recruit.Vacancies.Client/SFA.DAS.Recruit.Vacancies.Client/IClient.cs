using SFA.DAS.Recruit.Vacancies.Client.Entities;

namespace SFA.DAS.Recruit.Vacancies.Client
{
    public interface IClient
    {
        LiveVacancy GetVacancy(long vacancyReference);
    }
}