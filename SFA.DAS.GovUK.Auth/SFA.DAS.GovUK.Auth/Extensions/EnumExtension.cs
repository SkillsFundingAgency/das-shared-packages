using System;
using System.ComponentModel;
using System.Reflection;

namespace SFA.DAS.GovUK.Auth.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            return value
                .GetType()
                .GetField(value.ToString())
                ?.GetCustomAttribute<DescriptionAttribute>(false)
                ?.Description;
        }
    }
}
