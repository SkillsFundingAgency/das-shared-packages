using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Support.Shared
{
    [ExcludeFromCodeCoverage]
    public class PageLinks
    {
        public string Prev { get; set; }
        public string Self { get; set; }
        public string Next { get; set; }
    }
}