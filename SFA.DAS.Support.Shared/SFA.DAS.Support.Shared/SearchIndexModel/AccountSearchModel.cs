using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Support.Shared.SearchIndexModel
{
    [ExcludeFromCodeCoverage]
    public class AccountSearchModel: BaseSearchModel
    {
        public string Account { get; set; }

        public string AccountID { get; set; }

    }
}