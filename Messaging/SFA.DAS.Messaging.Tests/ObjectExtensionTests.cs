using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.Messaging.UnitTests
{
    [TestFixture]
    public class ObjectExtensionTests
    {
        [Test]
        public void ToDictionary()
        {
            var testClass = new TestProperties1();
            var dictionary = testClass.ToDictionary();

            Assert.IsTrue(dictionary.ContainsKey(nameof(TestProperties1.IntProperty)));
            Assert.IsTrue(dictionary.ContainsKey(nameof(TestProperties1.StringProperty)));
            Assert.IsTrue(dictionary.ContainsKey(nameof(TestProperties1.ReadOnlyProperty)));
        }
    }

    internal class TestProperties1
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
        public DateTime ReadOnlyProperty => DateTime.Today;
    }
}
