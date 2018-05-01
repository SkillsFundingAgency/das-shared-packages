namespace SFA.DAS.Recruit.Vacancies.Client
{
    public static class VacancyVersionHelper
    {
        private const int V2VacancyIdLength = 10;

        public static bool IsRecruitVacancy(long vacancyId)
        {
            return vacancyId.ToString("D").Length == V2VacancyIdLength;
        }

        public static bool IsRaaVacancy(long vacancyId)
        {
            return !(IsRecruitVacancy(vacancyId));
        }
    }
}
