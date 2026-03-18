using SFA.DAS.SharedOuterApi.Extensions;
using System.Collections.Specialized;

namespace SFA.DAS.SharedOuterApi.UnitTests.Extensions
{
    public class WhenGettingQueryString
    {
        [TestCase("pOne", "vOne", "", "", "", "")]
        [TestCase("pOne", "vOne", "pTwo", "vTwo", "pThree", "vThree")]
        [TestCase("pOne", "vOne", "pOne", "vTwo", "pThree", "vThree")]
        [TestCase("pOne", "vOne", "pOne", "vTwo", "pOne", "vThree")]
        public void Then_Returns_WelFormed_QueryString_From_NameValueCollection(string nameOne, string valueOne, string nameTwo, string valueTwo, string nameThree, string valueThree)
        {
            var nameValueCollection = new NameValueCollection();
            if (!string.IsNullOrEmpty(nameOne))
                nameValueCollection.Add(nameOne, valueOne);
            if (!string.IsNullOrEmpty(nameTwo))
                nameValueCollection.Add(nameTwo, valueTwo);
            if (!string.IsNullOrEmpty(nameThree))
                nameValueCollection.Add(nameThree, valueThree);

            nameValueCollection.ToQueryString().Should().MatchRegex("^\\?([\\w-]+(=[\\w-]*)?(&[\\w-]+(=[\\w-]*)?)*)?$");
        }

        [Test]
        public void With_Empty_NameValueCollection_Then_Returns_Empty()
        {
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.ToQueryString().Should().BeEmpty();
        }
    }
}