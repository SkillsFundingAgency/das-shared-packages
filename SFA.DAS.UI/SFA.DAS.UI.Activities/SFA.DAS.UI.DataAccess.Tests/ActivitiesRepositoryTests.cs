using System;
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

        [SetUp]
        public void Init()
        {
            _repo=new ActivitiesRepository(new ActivitiesConfiguration());
            
        }

        [Test]
        public void TenActivitiesAreReturnsPopulatedAsExprected()
        {
            var result = _repo.GetActivities("OwnerId").ToList();
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual("ActivityOne", result.First().Type);
            //Assert.IsTrue(result.All(a=>a.Type!=result.First().Type));
            Assert.AreEqual(4, result.GroupBy(a=>a.Type).Count());
        }
    }
}
