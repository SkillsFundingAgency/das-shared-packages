using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Support.Shared.SearchIndexModel
{
    [ExcludeFromCodeCoverage]
    public class UserSearchModel : BaseSearchModel
    {
        public string Id { get; set; }
        public string Name
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        public string Email { get; set; }
        public string Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}