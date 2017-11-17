using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using NuGet;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
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
        public void TenActivitiesAreReturnsPopulatedAsExprected()
        {
            var result = _repo.GetActivities(AccountId).ToList();
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual(Activity.ActivityTypeStrings.PayeSchemeCreated, result.First().TypeOfActivity);
            //Assert.IsTrue(result.All(a=>a.Type!=result.First().Type));
            Assert.AreEqual(4, result.GroupBy(a=>a.TypeOfActivity).Count());
        }

       
    }
}
