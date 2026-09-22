using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.GovUK.SampleSite.Configuration
{
    [ExcludeFromCodeCoverage]
    public class SampleSiteConfiguration
    {
        public required string OneLoginSettingsUrl { get; set; }
    }
}
