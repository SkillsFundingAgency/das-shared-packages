using System.Linq;
using NuGet;
using NUnit.Framework;
using SFA.DAS.UI.Activities.DataAccess.Repositories;
using SFA.DAS.UI.Activities.Domain.Configurations;

namespace SFA.DAS.UI.DataAccess.Tests
{
    public class ActivitiesRepositoryTests
    {
        private IActivitiesUiRepository _repo;
        private const long AccountId = 1234;
        

        [SetUp]
        public void Init()
        {
            _repo=new ActivitiesRepository(new ActivitiesConfiguration());
            //var elasticPopulator = new ElasticPopulator();
        }

        [Test]
        public void TenActivitiesAreReturnsPopulatedAsExpected()
        {
            var result = _repo.GetActivities(AccountId).ToList();
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual(3, result.Count(a=>a.TypeOfActivity.Equals(Activity.ActivityTypeStrings.AgreementSigned)));
            Assert.AreEqual(3, result.Count(a => a.TypeOfActivity.Equals(Activity.ActivityTypeStrings.AccountCreated)));
            Assert.AreEqual(3, result.GroupBy(a=>a.TypeOfActivity).Count());
        }

       
    }
}
