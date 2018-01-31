using System;
using NLog.Config;
using NLog.Layouts;

namespace SFA.DAS.NLog.Targets.Redis.DotNetCore
{
    [NLogConfigurationItem]
    public class Field
    {
        [RequiredParameter]
        public string Name { get; set; }

        [RequiredParameter]
        public Layout Layout { get; set; } 

        public Type LayoutType { get; set; } = typeof(string);

        public override string ToString()
        {
            return $"Name: {Name}, LayoutType: {LayoutType}, Layout: {Layout}";
        }
    }
}