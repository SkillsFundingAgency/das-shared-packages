using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using NUnit.Framework;
using SFA.DAS.UI.Activities.Areas.ActivitiesList.Mappers;
using SFA.DAS.UI.Activities.Areas.ActivitiesList.Models;

namespace SFA.DAS.UI.Activities.Web.Tests
{
    public class ActivityMapperTests
    {
        private ActivityMapper _mapper;

        private DateTime _testTime;
        private List<ActivitiesGroupedModel> _result;

        private DateTime _today13;
        private DateTime _today12;
        private DateTime _today11;
        private DateTime _today10;
        private DateTime _today09;
        private DateTime _today08;
        private DateTime _today07;
        private DateTime _today06;
        private DateTime _today05;
        private DateTime _today04;
        private DateTime _today03;
        private DateTime _today02;

        private DateTime _yesterday13;
        private DateTime _yesterday12;
        private DateTime _yesterday11;
        private DateTime _yesterday10;
        private DateTime _yesterday09;
        private DateTime _yesterday08;
        private DateTime _yesterday07;
        private DateTime _yesterday06;
        private DateTime _yesterday05;
        private DateTime _yesterday04;
        private DateTime _yesterday03;
        private DateTime _yesterday02;

        private DateTime _twoDaysAgo13;
        private DateTime _twoDaysAgo12;
        private DateTime _twoDaysAgo11;
        private DateTime _twoDaysAgo10;
        private DateTime _twoDaysAgo09;
        private DateTime _twoDaysAgo08;
        private DateTime _twoDaysAgo07;
        private DateTime _twoDaysAgo06;
        private DateTime _twoDaysAgo05;
        private DateTime _twoDaysAgo04;
        private DateTime _twoDaysAgo03;
        private DateTime _twoDaysAgo02;

        [SetUp]
        public void Init()
        {
            _mapper=new ActivityMapper();

            SetupTimes();

            var activities = MakeSomeActivities();

            _result = _mapper.SummariseCollections(activities).ToList();
        }

        [Test]
        public void TheTheGroupingsAreOnePerType()
        {
            Assert.AreEqual(5,_result.Count);
            Assert.IsTrue(_result.Exists(a => a.ActivityType == Activity.ActivityType.ActivityOne));
            Assert.IsTrue(_result.Exists(a => a.ActivityType == Activity.ActivityType.ActivityTwo));
            Assert.IsTrue(_result.Exists(a => a.ActivityType == Activity.ActivityType.ActivityThree));
            Assert.IsTrue(_result.Exists(a => a.ActivityType == Activity.ActivityType.ActivityFour));
            Assert.IsTrue(_result.Exists(a => a.ActivityType == Activity.ActivityType.ActivityFive));
        }

        private List<Activity> MakeSomeActivities()
        {
            List<Activity> rtn = new List<Activity>
            {
                MakeActivity(Activity.ActivityType.ActivityOne, _today02),
                MakeActivity(Activity.ActivityType.ActivityOne, _today03),
                MakeActivity(Activity.ActivityType.ActivityOne, _today02),
                MakeActivity(Activity.ActivityType.ActivityOne, _today03),

                MakeActivity(Activity.ActivityType.ActivityTwo, _today10),
                MakeActivity(Activity.ActivityType.ActivityTwo, _today03),
                MakeActivity(Activity.ActivityType.ActivityTwo, _today04),

                MakeActivity(Activity.ActivityType.ActivityThree, _today13),
                MakeActivity(Activity.ActivityType.ActivityThree, _today03),
                MakeActivity(Activity.ActivityType.ActivityThree, _twoDaysAgo12),
                MakeActivity(Activity.ActivityType.ActivityThree, _twoDaysAgo03),

                MakeActivity(Activity.ActivityType.ActivityFour, _today12),
                MakeActivity(Activity.ActivityType.ActivityFour, _today03),

                MakeActivity(Activity.ActivityType.ActivityFive, _yesterday11),
                MakeActivity(Activity.ActivityType.ActivityFive, _today13),


            };



            return rtn;
        }

        private Activity MakeActivity(string type, DateTime postedDateTime)
        {
            return new FluentActivity()
                .ActivityType(type)
                .PostedDateTime(postedDateTime)
                .Object();
        }

        private void SetupTimes()
        {
            _testTime = DateTime.Parse("2017/10/20")
                .AddHours(14);

            _today13 = _testTime.Subtract(new TimeSpan(0, 1, 0, 0));
            _today12 = _testTime.Subtract(new TimeSpan(0, 2, 0, 0));
            _today11 = _testTime.Subtract(new TimeSpan(0, 3, 0, 0));
            _today10 = _testTime.Subtract(new TimeSpan(0, 4, 0, 0));
            _today09 = _testTime.Subtract(new TimeSpan(0, 5, 0, 0));
            _today08 = _testTime.Subtract(new TimeSpan(0, 6, 0, 0));
            _today07 = _testTime.Subtract(new TimeSpan(0, 7, 0, 0));
            _today06 = _testTime.Subtract(new TimeSpan(0, 8, 0, 0));
            _today05 = _testTime.Subtract(new TimeSpan(0, 9, 0, 0));
            _today04 = _testTime.Subtract(new TimeSpan(0, 10, 0, 0));
            _today03 = _testTime.Subtract(new TimeSpan(0, 11, 0, 0));
            _today02 = _testTime.Subtract(new TimeSpan(0, 12, 0, 0));

            _yesterday13 = _testTime.Subtract(new TimeSpan(1, 1, 0, 0));
            _yesterday12 = _testTime.Subtract(new TimeSpan(1, 2, 0, 0));
            _yesterday11 = _testTime.Subtract(new TimeSpan(1, 3, 0, 0));
            _yesterday10 = _testTime.Subtract(new TimeSpan(1, 4, 0, 0));
            _yesterday09 = _testTime.Subtract(new TimeSpan(1, 5, 0, 0));
            _yesterday08 = _testTime.Subtract(new TimeSpan(1, 6, 0, 0));
            _yesterday07 = _testTime.Subtract(new TimeSpan(1, 7, 0, 0));
            _yesterday06 = _testTime.Subtract(new TimeSpan(1, 8, 0, 0));
            _yesterday05 = _testTime.Subtract(new TimeSpan(1, 9, 0, 0));
            _yesterday04 = _testTime.Subtract(new TimeSpan(1, 10, 0, 0));
            _yesterday03 = _testTime.Subtract(new TimeSpan(1, 11, 0, 0));
            _yesterday02 = _testTime.Subtract(new TimeSpan(1, 12, 0, 0));

            _twoDaysAgo13 = _testTime.Subtract(new TimeSpan(2, 1, 0, 0));
            _twoDaysAgo12 = _testTime.Subtract(new TimeSpan(2, 2, 0, 0));
            _twoDaysAgo11 = _testTime.Subtract(new TimeSpan(2, 3, 0, 0));
            _twoDaysAgo10 = _testTime.Subtract(new TimeSpan(2, 4, 0, 0));
            _twoDaysAgo09 = _testTime.Subtract(new TimeSpan(2, 5, 0, 0));
            _twoDaysAgo08 = _testTime.Subtract(new TimeSpan(2, 6, 0, 0));
            _twoDaysAgo07 = _testTime.Subtract(new TimeSpan(2, 7, 0, 0));
            _twoDaysAgo06 = _testTime.Subtract(new TimeSpan(2, 8, 0, 0));
            _twoDaysAgo05 = _testTime.Subtract(new TimeSpan(2, 9, 0, 0));
            _twoDaysAgo04 = _testTime.Subtract(new TimeSpan(2, 10, 0, 0));
            _twoDaysAgo03 = _testTime.Subtract(new TimeSpan(2, 11, 0, 0));
            _twoDaysAgo02 = _testTime.Subtract(new TimeSpan(2, 12, 0, 0));
        }
    }
}
