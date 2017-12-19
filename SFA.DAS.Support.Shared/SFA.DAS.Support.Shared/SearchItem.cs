using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Support.Shared
{
    [ExcludeFromCodeCoverage]
    public class SearchItem
    {
        public string SearchId { get; set; }

        public string[] Keywords { get; set; }

        public string Html { get; set; }
    }
}