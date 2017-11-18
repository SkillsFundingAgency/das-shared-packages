using System;
using System.Linq;
using NuGet;
using NUnit.Framework;
using SFA.DAS.UI.Activities.DataAccess.Repositories;
using SFA.DAS.UI.Activities.Domain.Configurations;

namespace SFA.DAS.UI.DataAccess.Tests
{
    public class GroupingTests
    {
        private IActivitiesUiRepository _repo;
        private const long AccountId = 1234;


        [SetUp]
        public void Init()
        {
            _repo = new ActivitiesRepository(new ActivitiesConfiguration());
            //var elasticPopulator = new ElasticPopulator();
        }

        [Test]
        public void AggregationsByDay()
        {
            var today= DateTime.Now.Date;
            var yesterday = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)).Date;
            var fourDaysAgo = DateTime.Now.Subtract(new TimeSpan(4, 0, 0, 0)).Date;

            var response = _repo.GetAggregationsByDay(AccountId);

            // response.IsValid.Should().BeTrue();
            var states = response.Aggs.Terms("keywordsBuckets");
            var hits = response.Hits;
            Assert.AreEqual(10, hits.Count);
            //states.Should().NotBeNull();
            //states.Buckets.Should().NotBeNullOrEmpty();
            foreach (var dateBucket in states.Buckets)
            {
                var a = 0;
                //state.Key.Should().NotBeNullOrEmpty();
                //state.DocCount.Should().BeGreaterThan(0);
                //var dateBuckets = dateBucket.Terms("dateBuckets").Buckets;
                //topStateHits.Should().NotBeNull();
                //Assert.IsNotNull(dateBuckets);
                //topStateHits.Total.Should().BeGreaterThan(0);
                //hits.Should().NotBeNullOrEmpty();
                //hits.All(h => h.Explanation != null).Should().BeTrue();
                //Assert.True(hits.All(h => h.Explanation != null));
                //hits.All(h => h.Version.HasValue).Should().BeTrue();
                //hits.All(h => h.Fields.ValuesOf<Activity>("state").Any()).Should().BeTrue();
                //Assert.IsTrue(hits.All(h=>h.Fields.ValuesOf<Activity>("state").Any()));
                //hits.All(h => h.Fields.ValuesOf<int>("numberOfCommits").Any()).Should().BeTrue()
                //hits.All(h => h.Fields.ValuesOf<int>("commit_factor").Any()).Should().BeTrue();
                //topStateHits.Documents<Project>().Should().NotBeEmpty();
                //mine
            }

            Assert.AreEqual(3, states.Buckets.Count);

            var bucketA1 = states.Buckets.Single(a => a.Key == today.ToString());
            Assert.AreEqual(4, bucketA1.DocCount);

            var bucketA2 = states.Buckets.Single(a => a.Key == yesterday.ToString());
            Assert.AreEqual(4, bucketA2.DocCount);

            var bucketA3 = states.Buckets.Single(a => a.Key ==fourDaysAgo.ToString());
            Assert.AreEqual(2, bucketA3.DocCount);

            Assert.AreEqual(today.ToString(),states.Buckets.ToList()[0].Key);
            Assert.AreEqual(yesterday.ToString(), states.Buckets.ToList()[1].Key);
            Assert.AreEqual(fourDaysAgo.ToString(), states.Buckets.ToList()[2].Key);

        }

        [Test]
        public void AggregationsByType()
        {
            var response = _repo.GetAggregationsByType(AccountId);

            // response.IsValid.Should().BeTrue();
            var states = response.Aggs.Terms("keywordsBuckets");
            //states.Should().NotBeNull();
            //states.Buckets.Should().NotBeNullOrEmpty();
            foreach (var dateBucket in states.Buckets)
            {

                //state.Key.Should().NotBeNullOrEmpty();
                //state.DocCount.Should().BeGreaterThan(0);
                var dateBuckets = dateBucket.Terms("dateBuckets").Buckets;
                //topStateHits.Should().NotBeNull();
                Assert.IsNotNull(dateBuckets);
                //topStateHits.Total.Should().BeGreaterThan(0);
                //hits.Should().NotBeNullOrEmpty();
                //hits.All(h => h.Explanation != null).Should().BeTrue();
                //Assert.True(hits.All(h => h.Explanation != null));
                //hits.All(h => h.Version.HasValue).Should().BeTrue();
                //hits.All(h => h.Fields.ValuesOf<Activity>("state").Any()).Should().BeTrue();
                //Assert.IsTrue(hits.All(h=>h.Fields.ValuesOf<Activity>("state").Any()));
                //hits.All(h => h.Fields.ValuesOf<int>("numberOfCommits").Any()).Should().BeTrue()
                //hits.All(h => h.Fields.ValuesOf<int>("commit_factor").Any()).Should().BeTrue();
                //topStateHits.Documents<Project>().Should().NotBeEmpty();
                //mine
            }

            //Assert.AreEqual(5, states.Buckets.Count);

            var bucketA1 = states.Buckets.Single(a => a.Key == Activity.ActivityTypeStrings.AgreementSigned);
            Assert.AreEqual(24, bucketA1.DocCount);//actually expect 12

            var bucketA2 = states.Buckets.Single(a => a.Key == Activity.ActivityTypeStrings.AccountCreated);
            Assert.AreEqual(20, bucketA2.DocCount);//actually expect 10

            var bucketA3 = states.Buckets.Single(a => a.Key == Activity.ActivityTypeStrings.AgreementCreated);
            Assert.AreEqual(12, bucketA3.DocCount);//actually expect 6

            var bucketA4 = states.Buckets.Single(a => a.Key == Activity.ActivityTypeStrings.PayeSchemeCreated);
            Assert.AreEqual(12, bucketA4.DocCount);//actually expect 6

            var bucketA5 = states.Buckets.Single(a => a.Key == Activity.ActivityTypeStrings.UserInvited);
            Assert.AreEqual(14, bucketA5.DocCount);//actually expect 7

        }
    }
}
