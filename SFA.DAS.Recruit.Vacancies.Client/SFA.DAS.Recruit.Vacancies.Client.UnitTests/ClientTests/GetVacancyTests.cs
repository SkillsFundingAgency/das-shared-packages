using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.Recruit.Vacancies.Client.UnitTests.ClientTests
{
    [TestFixture]
    public class GetVacancyTests
    {
        private readonly string connectionString = "";
        [Ignore("A helper test to get vacancy")]
        [Test]
        public void ShouldGetVacancyFromLiveOrClosedView()
        {
            var sut = new Client(connectionString, "recruit", "queryStore", null);

            var vacancyReference = 1000000022;
            var vac = sut.GetPublishedVacancy(vacancyReference);

            Assert.AreEqual(vacancyReference, vac.VacancyReference);
        }

        [Ignore("A helper test to get live vacancy")]
        [Test]
        public void ShouldGetVacancyFromLiveView()
        {
            var sut = new Client(connectionString, "recruit", "queryStore", null);

            var vacancyReference = 1000005528;
            var vac = sut.GetLiveVacancy(vacancyReference);

            Assert.AreEqual(vacancyReference, vac.VacancyReference);
        }

        [Ignore("A helper test to get all live vacancies in pages")]
        [Test]
        public async Task ShouldGetPagedVacancies()
        {
            var sut = new Client(connectionString, "recruit", "queryStore", null);

            var count = await sut.GetLiveVacanciesCount();

            var pages = (count / 4) + 1;
            var i = 0;
            var retrievedCount = 0;
            while(i < pages)
            {
                i++;
                var vac = await sut.GetLiveVacanciesAsync(4,i);
                retrievedCount += vac.Count;
            }

            Assert.AreEqual(count, retrievedCount);
        }

    }
}
