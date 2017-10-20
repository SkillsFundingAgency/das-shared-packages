using System;
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
            return GetMessageGroupName(typeof(T));
        }

        public static string GetMessageGroupName(Type type)
        {
            var subscriptionName = (string)type.CustomAttributes
                .FirstOrDefault(att => att.AttributeType.Name == nameof(TopicSubscriptionAttribute))
                ?.ConstructorArguments.FirstOrDefault().Value;

            return string.IsNullOrEmpty(subscriptionName) ? type.Name : subscriptionName;
        }
    }
}
