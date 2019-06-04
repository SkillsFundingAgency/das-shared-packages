namespace SFA.DAS.Authorization
{
    public class AuthorizationContext : IAuthorizationContext
    {
        public IAccountContext AccountContext { get; set; }
        public IMembershipContext MembershipContext { get; set; }
        public IUserContext UserContext { get; set; }
    };
}