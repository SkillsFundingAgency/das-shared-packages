using System.Collections.Generic;
using System.Linq;
using NuGet;
using NUnit.Framework;
using SFA.DAS.UI.Activities.Areas.ActivitiesList.Mappers;

namespace DAS.UI.Activities.Web.Tests.Mappers.ActivityMapperTests
{
    public class WhenActivitiesOfManyTypesArePresented
    {
        private ActivityMapper _mapper;

        private List<Activity> _activities;

        private const string OwnerId = "Mr Horse";

        private const string DescriptionSingularOne = "desc singular 1";
        private const string DescriptionPluralOne = "desc plural 1";
        private const string DescriptionSingularTwo = "desc singular 2";
        private const string DescriptionPluralTwo = "desc plural 2";
        private const string DescriptionSingularThree = "desc singular 3";
        private const string DescriptionPluralThree = "desc plural 3";

        [SetUp]
        public void Init()
        {
            _mapper=new ActivityMapper();

            _activities = new List<Activity>()
            {
                new FluentActivity().ActivityType(Activity.ActivityType.ActivityOne)
                    .DescriptionPlural(DescriptionPluralOne).DescriptionSingular(DescriptionSingularOne).OwnerId(OwnerId).Object(),
                new FluentActivity().ActivityType(Activity.ActivityType.ActivityOne)
                    .DescriptionPlural(DescriptionPluralOne).DescriptionSingular(DescriptionSingularOne).OwnerId(OwnerId).Object(),
                new FluentActivity().ActivityType(Activity.ActivityType.ActivityOne)
                    .DescriptionPlural(DescriptionPluralOne).DescriptionSingular(DescriptionSingularOne).OwnerId(OwnerId).Object(),
                new FluentActivity().ActivityType(Activity.ActivityType.ActivityTwo)
                    .DescriptionPlural(DescriptionPluralTwo).DescriptionSingular(DescriptionSingularTwo).OwnerId(OwnerId).Object(),
                new FluentActivity().ActivityType(Activity.ActivityType.ActivityTwo)
                    .DescriptionPlural(DescriptionPluralTwo).DescriptionSingular(DescriptionSingularTwo).OwnerId(OwnerId).Object(),
                new FluentActivity().ActivityType(Activity.ActivityType.ActivityThree)
                    .DescriptionPlural(DescriptionPluralThree).DescriptionSingular(DescriptionSingularThree).OwnerId(OwnerId).Object()
            };
        }

        [Test]
        public void ThenTheGroupingHasTheCorrectCount()
        {
            var result = _mapper.SummariseCollections(_activities);

            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public void ThenTheGroupingsHaveTheCorrectText()
        {
            var result = _mapper.SummariseCollections(_activities).ToList();

            Assert.AreEqual($"3 {DescriptionPluralOne}", result.Single(a => a.ActivityType == Activity.ActivityType.ActivityOne).Description);
            Assert.AreEqual(string.Empty, result.Single(a => a.ActivityType == Activity.ActivityType.ActivityOne).ByWhomText);
            Assert.AreEqual($"2 {DescriptionPluralTwo}", result.Single(a => a.ActivityType == Activity.ActivityType.ActivityTwo).Description);
            Assert.AreEqual(string.Empty, result.Single(a => a.ActivityType == Activity.ActivityType.ActivityTwo).ByWhomText);
            Assert.AreEqual($"1 {DescriptionSingularThree}", result.Single(a => a.ActivityType == Activity.ActivityType.ActivityThree).Description);
            Assert.AreEqual($"By {OwnerId}", result.Single(a => a.ActivityType == Activity.ActivityType.ActivityThree).ByWhomText);
        }

    }
}
