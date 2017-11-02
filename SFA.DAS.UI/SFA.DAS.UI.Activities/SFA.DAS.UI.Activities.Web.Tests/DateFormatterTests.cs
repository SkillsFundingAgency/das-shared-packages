using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.UI.Activities.Areas.ActivitiesPage;

namespace SFA.DAS.UI.Activities.Web.Tests
{
    public class DateFormatterTests
    {

        [Test]
        public void Today()
        {
            var result = DateFormatter.CreateDayText(DateTime.Now);
            Assert.AreEqual("Today",result);
        }

        [Test]
        public void Yeserday()
        {
            var result = DateFormatter.CreateDayText(DateTime.Now.Subtract(new TimeSpan(1,0,0,0)));
            Assert.AreEqual("Yesterday", result);
        }

        [Test]
        public void ADateInThePastWithDayOver12()
        {
            var testDateTime = DateTime.Parse("2017/10/14").Add(new TimeSpan(0, 6, 7, 8));
            var result = DateFormatter.CreateDayText(testDateTime);
            Assert.AreEqual("2017/10/14", result);
        }

        [Test]
        public void ADateInThePastWithDayLessThan10()
        {
            var testDateTime = DateTime.Parse("2017/10/08").Add(new TimeSpan(0,6,7,8));
            var result = DateFormatter.CreateDayText(testDateTime);
            Assert.AreEqual("2017/10/08", result);
        }
    }
}
