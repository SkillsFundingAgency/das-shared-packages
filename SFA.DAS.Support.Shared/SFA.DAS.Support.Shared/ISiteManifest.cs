using System.Collections.Generic;

namespace SFA.DAS.Support.Shared
{
    public interface ISiteManifest
    {
        string Version { get; }
        IEnumerable<SiteResource> Resources { get; }
        string BaseUrl { get; }
        IEnumerable<SiteChallenge> Challenges { get; }
    }
}