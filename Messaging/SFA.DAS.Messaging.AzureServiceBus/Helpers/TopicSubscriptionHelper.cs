using System.Linq;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;

namespace SFA.DAS.Messaging.AzureServiceBus.Helpers
{
    public class TopicSubscriptionHelper
    {
        public static string GetMessageGroupName(object obj)
        {
            var subscriptionName = (string)obj.GetType()
                .CustomAttributes
                .FirstOrDefault(att => att.AttributeType.Name == nameof(TopicSubscriptionAttribute))
                ?.ConstructorArguments.FirstOrDefault().Value;

            return string.IsNullOrEmpty(subscriptionName) ? obj.GetType().Name : subscriptionName;
        }

        public static string GetMessageGroupName<T>()
        {
            var subscriptionName = (string)typeof(T).CustomAttributes
                .FirstOrDefault(att => att.AttributeType.Name == nameof(TopicSubscriptionAttribute))
                ?.ConstructorArguments.FirstOrDefault().Value;

            return string.IsNullOrEmpty(subscriptionName) ? typeof(T).Name : subscriptionName;
        }
    }
}
