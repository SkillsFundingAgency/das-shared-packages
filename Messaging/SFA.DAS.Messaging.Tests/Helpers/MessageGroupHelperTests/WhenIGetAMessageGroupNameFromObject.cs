using NUnit.Framework;
using SFA.DAS.Messaging.Attributes;
using SFA.DAS.Messaging.Helper;

namespace SFA.DAS.Messaging.UnitTests.Helpers.MessageGroupHelperTests
{
    public class WhenIGetAMessageGroupNameFromObject
    {
        [Test]
        public void ShouldGetGroupNameIfOneExists()
        {
            var groupName = MessageGroupHelper.GetMessageGroupName(new TestClassWithGroupName());

            Assert.AreEqual("Test", groupName);
        }

        [Test]
        public void ShouldReturnClassNameIfGroupNameNotFound()
        {
            var groupName = MessageGroupHelper.GetMessageGroupName(new TestClassWithoutGroupName());

            Assert.AreEqual(nameof(TestClassWithoutGroupName), groupName);
        }

        [Test]
        public void ShouldReturnClassNameIfGroupNameIsEmpty()
        {
            var groupName = MessageGroupHelper.GetMessageGroupName(new TestClassWithEmptyGroupName());

            Assert.AreEqual(nameof(TestClassWithEmptyGroupName), groupName);
        }



        [MessageGroup("Test")]
        private class TestClassWithGroupName
        {   }

        [MessageGroup()]
        private class TestClassWithEmptyGroupName
        {   }

        private class TestClassWithoutGroupName
        {   }
    }
}
