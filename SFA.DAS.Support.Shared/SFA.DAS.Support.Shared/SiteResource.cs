using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Support.Shared
{
    [ExcludeFromCodeCoverage]
    public class SiteResource
    {
        public string ResourceKey { get; set; }
        public string ResourceUrlFormat { get; set; }
        public string SearchItemsUrl { get; set; }
        public string ResourceTitle { get; set; }
        public string Challenge { get; set; }
    }
}