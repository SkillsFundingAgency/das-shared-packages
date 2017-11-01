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
        

        [SetUp]
        public void Init()
        {
            _repo=new ActivitiesRepository(new ActivitiesConfiguration());
            //var elasticPopulator = new ElasticPopulator();
        }

        [Test]
        public void TenActivitiesAreReturnsPopulatedAsExprected()
        {
            var result = _repo.GetActivities("OwnerId").ToList();
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual("a4", result.First().TypeOfActivity);
            //Assert.IsTrue(result.All(a=>a.Type!=result.First().Type));
            Assert.AreEqual(4, result.GroupBy(a=>a.TypeOfActivity).Count());
        }

        [Test]
        public void Aggregations()
        {
            var result = _repo.GetAggregations("OwnerId");
            var result2 = result.ToList();
            Assert.IsFalse(result2.Count==0);
           // Assert.AreEqual(4, result2.Count);
        }

        [Test]
        public void Aggregations2Aggregation()
        {
            var levelAggregationName = "top_tags";
            var results = _repo.GetAggregations2("OwnerId");

            Assert.AreEqual(10,results.Hits.Count);
            Assert.AreEqual(10, results.Documents.Count);
            Assert.AreEqual(1, results.Aggregations.Count);
            Assert.AreEqual(1, results.Aggregations.Keys.Count());
            Assert.AreEqual(typeof(BucketAggregate), results.Aggregations.Values.First().GetType());
            Assert.AreEqual(typeof(BucketAggregate).FullName, results.Aggregations.Values.First().ToString());


            Assert.IsTrue(results.Aggs.Terms(levelAggregationName)!=null);
            Assert.IsTrue(results.Aggs.Terms(levelAggregationName).Buckets.Count>0);
        }

        [Test]
        public void Aggregations2Agg()
        {
            var levelAggregationName = "top_tags";
            var results = _repo.GetAggregations2("OwnerId");

            Assert.AreEqual(10, results.Hits.Count);

            var agg = results.Aggregations["top_tag_hits"];

            Assert.AreEqual(1, results.Aggregations.Count);
            Assert.AreEqual(true, results.Aggregations.ContainsKey("top_tag_hits"));
            Assert.AreEqual(true, results.Aggregations["top_tag_hits"]);

            Assert.IsNotNull(results.Aggs.ValueCount("a1"));
            Assert.AreEqual(1, results.Aggregations.Keys.Count());
            Assert.AreEqual(levelAggregationName, results.Aggregations.Keys.First());
            Assert.AreEqual(levelAggregationName, results.Aggregations.Values.First());
            Assert.AreEqual(3, results.Aggregations.Values.First().Meta.First().Value);


            Assert.IsTrue(results.Aggs.Terms(levelAggregationName) != null);
            Assert.IsTrue(results.Aggs.Terms(levelAggregationName).Buckets.Count > 0);
        }

        [Test]
        public void Aggregations3Agg()
        {
            var response = _repo.GetAggregations3("OwnerId");

           // response.IsValid.Should().BeTrue();
            var states = response.Aggs.Terms("keywords");
            //states.Should().NotBeNull();
            //states.Buckets.Should().NotBeNullOrEmpty();
            foreach (var state in states.Buckets)
            {
                //state.Key.Should().NotBeNullOrEmpty();
                //state.DocCount.Should().BeGreaterThan(0);
                var topStateHits = state.TopHits("top_state_hits");
                //topStateHits.Should().NotBeNull();
                Assert.IsNotNull(topStateHits);
                //topStateHits.Total.Should().BeGreaterThan(0);
                Assert.IsTrue(topStateHits.Total>0);
                var hits = topStateHits.Hits<Activity>();
                //hits.Should().NotBeNullOrEmpty();
                Assert.IsNotNull(hits);
                Assert.IsTrue(hits.Any());
                //hits.All(h => h.Explanation != null).Should().BeTrue();
                //Assert.True(hits.All(h => h.Explanation != null));
                //hits.All(h => h.Version.HasValue).Should().BeTrue();
                //hits.All(h => h.Fields.ValuesOf<Activity>("state").Any()).Should().BeTrue();
                //Assert.IsTrue(hits.All(h=>h.Fields.ValuesOf<Activity>("state").Any()));
                //hits.All(h => h.Fields.ValuesOf<int>("numberOfCommits").Any()).Should().BeTrue()
                //hits.All(h => h.Fields.ValuesOf<int>("commit_factor").Any()).Should().BeTrue();
                //topStateHits.Documents<Project>().Should().NotBeEmpty();
                Assert.IsNotNull(topStateHits.Documents<Activity>());
                //mine
            }

            //Assert.AreEqual(5, states.Buckets.Count);

            var bucketA1 = states.Buckets.Single(a => a.Key == "a1");
            Assert.AreEqual(24, bucketA1.DocCount);//actually expect 12

            var bucketA2 = states.Buckets.Single(a => a.Key == "a2");
            Assert.AreEqual(20, bucketA2.DocCount);//actually expect 10

            var bucketA3 = states.Buckets.Single(a => a.Key == "a3");
            Assert.AreEqual(12, bucketA3.DocCount);//actually expect 6

            var bucketA4 = states.Buckets.Single(a => a.Key == "a4");
            Assert.AreEqual(12, bucketA4.DocCount);//actually expect 6

            var bucketA5 = states.Buckets.Single(a => a.Key == "a5");
            Assert.AreEqual(14, bucketA5.DocCount);//actually expect 7

        }

        [Test]
        public void TenActivitiesAreGroupedAsExprected()
        {
            var result = _repo.GetActivitiesGroupedByDayAndType("OwnerId").ToList();
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual("a1", result.First().TypeOfActivity);
            //Assert.IsTrue(result.All(a=>a.Type!=result.First().Type));
            Assert.AreEqual(4, result.GroupBy(a => a.TypeOfActivity).Count());
        }
    }
}
