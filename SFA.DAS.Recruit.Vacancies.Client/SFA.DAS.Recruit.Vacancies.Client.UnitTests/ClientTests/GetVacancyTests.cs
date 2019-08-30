using NUnit.Framework;

namespace SFA.DAS.Recruit.Vacancies.Client.UnitTests.ClientTests
{
    [TestFixture]
    public class GetVacancyTests
    {
        [Ignore("A helper test to get vacancy")]
        [Test]
        public void ShouldGetVacancyFromLiveOrClosedView()
        {
            var connectionString = string.Empty;
            var sut = new Client(connectionString, "recruit", "queryStore", null);

            var vacancyReference = 1000000022;
            var vac = sut.GetPublishedVacancy(vacancyReference);

            Assert.AreEqual(vacancyReference, vac.VacancyReference);
        }

        [Ignore("A helper test to get live vacancy")]
        [Test]
        public void ShouldGetVacancyFromLiveView()
        {
            var connectionString = string.Empty;
            var sut = new Client(connectionString, "recruit", "queryStore", null);

            var vacancyReference = 1000005528;
            var vac = sut.GetLiveVacancy(vacancyReference);

            Assert.AreEqual(vacancyReference, vac.VacancyReference);
        }
    }
}
