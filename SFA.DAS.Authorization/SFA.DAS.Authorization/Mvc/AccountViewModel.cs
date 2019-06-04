using AutoMapper;

namespace SFA.DAS.Authorization.Mvc
{
    public abstract class AccountViewModel : IAccountViewModel
    {
        [IgnoreMap]
        public long AccountId { get; set; }

        [IgnoreMap]
        public string AccountHashedId { get; set; }
    }
}