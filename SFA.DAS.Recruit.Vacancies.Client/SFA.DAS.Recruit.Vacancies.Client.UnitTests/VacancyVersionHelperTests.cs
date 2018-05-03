using NUnit.Framework;

namespace SFA.DAS.Recruit.Vacancies.Client.UnitTests
{
    [TestFixture]
    public class VacancyVersionHelperTests
    {
        [Test]
        public void IsRecruitVacancy_ShouldReturnTrueForRecruitVacancies()
        {
            long vacancyReference = 1000000000;

            Assert.IsTrue(VacancyVersionHelper.IsRecruitVacancy(vacancyReference));
            Assert.IsFalse(VacancyVersionHelper.IsRaaVacancy(vacancyReference));
        }

        [Test]
        public void IsRecruitVacancy_ShouldReturnFalseForRecruitVacancies()
        {
            long vacancyReference = 100000000;

            Assert.IsFalse(VacancyVersionHelper.IsRecruitVacancy(vacancyReference));
            Assert.IsTrue(VacancyVersionHelper.IsRaaVacancy(vacancyReference));
        }
    }
}
