using System.Collections.Generic;

namespace SFA.DAS.Api.Common.Infrastructure
{
    public static class PolicyNames
    {
        public static string DataLoad => nameof(DataLoad);
        public static string Default => nameof(Default);
        
        public static readonly List<string> PolicyNameList = new List<string>
        {
            DataLoad
        };
    }
}