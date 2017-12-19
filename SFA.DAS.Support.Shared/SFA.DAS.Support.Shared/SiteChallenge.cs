using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Support.Shared
{
    [ExcludeFromCodeCoverage]
    public class SiteChallenge
    {
        public string ChallengeKey { get; set; }
        public string ChallengeUrlFormat { get; set; }
    }
}