using NUnit.Framework;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;

namespace SFA.DAS.Messaging.AzureServiceBus.UnitTests.Helpers.TopicSubscriptionHelperTests
{
    public class WhenIGetATopicSubscriptionNameFromObject
    {
        [Test]
        public void ShouldGetGroupNameIfOneExists()
        {
            var groupName = TopicSubscriptionHelper.GetMessageGroupName(new TestClassWithSubscriptionName());

            Assert.AreEqual("Test", groupName);
        }

        [Test]
        public void ShouldReturnClassNameIfGroupNameNotFound()
        {
            var groupName = TopicSubscriptionHelper.GetMessageGroupName(new TestClassWithoutSubscriptionName());

            Assert.AreEqual(nameof(TestClassWithoutSubscriptionName), groupName);
        }

        [Test]
        public void ShouldReturnClassNameIfGroupNameIsEmpty()
        {
            var groupName = TopicSubscriptionHelper.GetMessageGroupName(new TestClassWithEmptySubscriptionName());

            Assert.AreEqual(nameof(TestClassWithEmptySubscriptionName), groupName);
        }



        [TopicSubscription("Test")]
        private class TestClassWithSubscriptionName
        { }

        [TopicSubscription()]
        private class TestClassWithEmptySubscriptionName
        { }

        private class TestClassWithoutSubscriptionName
        { }
    }
}
