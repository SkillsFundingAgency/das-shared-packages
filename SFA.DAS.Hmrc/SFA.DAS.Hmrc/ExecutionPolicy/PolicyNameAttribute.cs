using System;

namespace SFA.DAS.Hmrc.ExecutionPolicy
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PolicyNameAttribute : Attribute
    {
        public PolicyNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}