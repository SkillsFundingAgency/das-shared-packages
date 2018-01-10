using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Support.Shared.SearchIndexModel
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseSearchModel
    {
        public SearchCategory SearchType { get; set; }
    }
}