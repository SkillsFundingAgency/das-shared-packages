﻿using System.Linq;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.Messaging.Helpers
{
    public class MessageGroupHelper
    {
        public static string GetMessageGroupName(object obj)
        {
            var groupName = (string) obj.GetType()
                .CustomAttributes
                .FirstOrDefault(att => att.AttributeType.Name == nameof(MessageGroupAttribute))
                ?.ConstructorArguments.FirstOrDefault().Value;

            return string.IsNullOrEmpty(groupName) ? obj.GetType().Name : groupName;
        }

        public static string GetMessageGroupName<T>()
        {
            var groupName = (string)typeof(T).CustomAttributes
                                .FirstOrDefault(att => att.AttributeType.Name == nameof(MessageGroupAttribute))
                                ?.ConstructorArguments.FirstOrDefault().Value;

            return string.IsNullOrEmpty(groupName) ? typeof(T).Name : groupName;
        }
    }
}
