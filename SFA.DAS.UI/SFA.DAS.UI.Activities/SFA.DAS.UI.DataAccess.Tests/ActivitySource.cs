using System;
using System.Collections.Generic;
using NuGet;

namespace SFA.DAS.UI.DataAccess.Tests
{
    public class ActivitySource
    {
        private DateTime _testTime;

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

        private DateTime _twoDaysAgo08;
        private DateTime _twoDaysAgo07;
        private DateTime _twoDaysAgo06;
        private DateTime _twoDaysAgo05;
        private DateTime _twoDaysAgo04;
        private DateTime _twoDaysAgo03;
        private DateTime _twoDaysAgo02;

        private DateTime _threeDaysAgo06;
        private DateTime _threeDaysAgo05;
        private DateTime _threeDaysAgo04;
        private DateTime _threeDaysAgo03;
        private DateTime _threeDaysAgo02;
        private DateTime _threeDaysAgo01;

        private DateTime _fourDaysAgo06;
        private DateTime _fourDaysAgo05;
        private DateTime _fourDaysAgo04;
        private DateTime _fourDaysAgo03;
        private DateTime _fourDaysAgo02;
        private DateTime _fourDaysAgo01;

        private const string ActivityOne = "a1";
        private const string ActivityTwo = "a2";
        private const string ActivityThree = "a3";
        private const string ActivityFour = "a4";
        private const string ActivityFive = "a5";

        public ActivitySource()
        {
            SetupTimes();
            TestActivities = MakeSomeActivities();
        }

        public List<Activity> TestActivities { get;  }

        private List<Activity> MakeSomeActivities()
        {
            List<Activity> rtn = new List<Activity>
            {
                //today:
                MakeActivity(ActivityOne, _today02),
                MakeActivity(ActivityOne, _today03),
                MakeActivity(ActivityOne, _today02),
                MakeActivity(ActivityOne, _today03),
                MakeActivity(ActivityOne, _today04),
                MakeActivity(ActivityOne, _today02),
                MakeActivity(ActivityOne, _today03),

                MakeActivity(ActivityTwo, _today10),
                MakeActivity(ActivityTwo, _today03),
                MakeActivity(ActivityTwo, _today04),
                MakeActivity(ActivityTwo, _today02),

                //yesterday:

                MakeActivity(ActivityOne, _yesterday11),
                MakeActivity(ActivityOne, _yesterday10),
                MakeActivity(ActivityOne, _yesterday09),
                MakeActivity(ActivityOne, _yesterday08),
                MakeActivity(ActivityOne, _yesterday07),

                MakeActivity(ActivityTwo, _yesterday06),
                MakeActivity(ActivityTwo, _yesterday05),
                MakeActivity(ActivityTwo, _yesterday04),
                MakeActivity(ActivityTwo, _yesterday03),
                MakeActivity(ActivityTwo, _yesterday02),
                MakeActivity(ActivityTwo, _yesterday02),

                MakeActivity(ActivityThree, _yesterday02),
                MakeActivity(ActivityThree, _yesterday03),

                MakeActivity(ActivityFour, _yesterday02),
                MakeActivity(ActivityFour, _yesterday03),

                MakeActivity(ActivityFive, _yesterday11),
                MakeActivity(ActivityFive, _yesterday07),

                //two days ago

                MakeActivity(ActivityThree, _twoDaysAgo08),
                MakeActivity(ActivityThree, _twoDaysAgo03),

                MakeActivity(ActivityFour, _today12),
                MakeActivity(ActivityFour, _today03),

                MakeActivity(ActivityFive, _yesterday11),
                MakeActivity(ActivityFive, _today13),

                //four days ago

                MakeActivity(ActivityThree, _fourDaysAgo06),
                MakeActivity(ActivityThree, _fourDaysAgo05),

                MakeActivity(ActivityFour, _fourDaysAgo04),
                MakeActivity(ActivityFour, _fourDaysAgo03),

                MakeActivity(ActivityFive, _fourDaysAgo02),
                MakeActivity(ActivityFive, _fourDaysAgo05),


            };



            return rtn;
        }

        private static Activity MakeActivity(string type, DateTime when)
        {
            return new FluentActivity()
                .ActivityType(type)
                .PostedDateTime(when)
                .DescriptionSingular("desc singular")
                .DescriptionPlural(" things happened")
                .DescriptionFull("A thing happened")
                .AddAssociatedThing("associated thing")
                .AddAssociatedThings(new[] { "dog", "cat", "horse" })
                .Url("todo")
                .OwnerId("OwnerId")
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

            _twoDaysAgo08 = _testTime.Subtract(new TimeSpan(2, 6, 0, 0));
            _twoDaysAgo07 = _testTime.Subtract(new TimeSpan(2, 7, 0, 0));
            _twoDaysAgo06 = _testTime.Subtract(new TimeSpan(2, 8, 0, 0));
            _twoDaysAgo05 = _testTime.Subtract(new TimeSpan(2, 9, 0, 0));
            _twoDaysAgo04 = _testTime.Subtract(new TimeSpan(2, 10, 0, 0));
            _twoDaysAgo03 = _testTime.Subtract(new TimeSpan(2, 11, 0, 0));
            _twoDaysAgo02 = _testTime.Subtract(new TimeSpan(2, 12, 0, 0));
            //

            _threeDaysAgo06 = _testTime.Subtract(new TimeSpan(3, 6, 0, 0));
            _threeDaysAgo05 = _testTime.Subtract(new TimeSpan(3, 7, 0, 0));
            _threeDaysAgo04 = _testTime.Subtract(new TimeSpan(3, 8, 0, 0));
            _threeDaysAgo03 = _testTime.Subtract(new TimeSpan(3, 9, 0, 0));
            _threeDaysAgo02 = _testTime.Subtract(new TimeSpan(3, 10, 0, 0));
            _threeDaysAgo01 = _testTime.Subtract(new TimeSpan(3, 11, 0, 0));

            _fourDaysAgo06 = _testTime.Subtract(new TimeSpan(4, 12, 0, 0));
            _fourDaysAgo05 = _testTime.Subtract(new TimeSpan(4, 6, 0, 0));
            _fourDaysAgo04 = _testTime.Subtract(new TimeSpan(4, 7, 0, 0));
            _fourDaysAgo03 = _testTime.Subtract(new TimeSpan(4, 8, 0, 0));
            _fourDaysAgo02 = _testTime.Subtract(new TimeSpan(4, 9, 0, 0));
            _fourDaysAgo01 = _testTime.Subtract(new TimeSpan(4, 10, 0, 0));

    }
    }
}
