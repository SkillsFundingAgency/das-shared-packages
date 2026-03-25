using System.Collections.Generic;
using System.Collections.Specialized;
using SFA.DAS.Apim.Shared.Extensions;

namespace SFA.DAS.Apim.Shared.UnitTests.Extensions
{
    public class WhenGettingNameValueCollection
    {
        private static readonly object[] _stringLists =
            {
                new object[] {new List<string> { "vOne" } },
                new object[] {new List<string> { "vOne", "vTwo", "vThree" }}
            };

        [TestCaseSource("_stringLists")]
        public void Then_Returns_NameValueCollection_With_Same_Key(List<string> valueOne)
        {
            string key = "Key";
            var collection = valueOne.ToNameValueCollection(key);

            Assert.That(collection, Is.InstanceOf<NameValueCollection>());
            for (int i = 0; i < collection.Count; i++)
            {
                Assert.That(collection.GetKey(i) == key, Is.True);
            }

        }
    }
}