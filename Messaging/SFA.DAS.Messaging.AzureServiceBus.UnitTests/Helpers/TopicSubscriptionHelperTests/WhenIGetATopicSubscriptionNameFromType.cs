using NUnit.Framework;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;

namespace SFA.DAS.Messaging.AzureServiceBus.UnitTests.Helpers.TopicSubscriptionHelperTests
{
    public class WhenIGetATopicSubscriptionNameFromType
    {
        [Test]
        public void ShouldGetGroupNameIfOneExists()
        {
            var groupName = TopicSubscriptionHelper.GetMessageGroupName< TestClassWithSubscriptionName>();

            Assert.AreEqual("Test", groupName);
        }

        [Test]
        public void ShouldReturnClassNameIfGroupNameNotFound()
        {
            var groupName = TopicSubscriptionHelper.GetMessageGroupName<TestClassWithoutSubscriptionName>();

            Assert.AreEqual(nameof(TestClassWithoutSubscriptionName), groupName);
        }

        [Test]
        public void ShouldReturnClassNameIfGroupNameIsEmpty()
        {
            var groupName = TopicSubscriptionHelper.GetMessageGroupName<TestClassWithEmptySubscriptionName>();

            Assert.AreEqual(nameof(TestClassWithEmptySubscriptionName), groupName);
        }

        [TopicSubscription("Test")]
        private class TestClassWithSubscriptionName
        {   }

        [TopicSubscription()]
        private class TestClassWithEmptySubscriptionName
        {   }

        private class TestClassWithoutSubscriptionName
        {   }
    }
}
