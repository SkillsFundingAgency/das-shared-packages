namespace SFA.DAS.Authorization
{
    public interface IAuthorizationContext
    {
        IAccountContext AccountContext { get; }
        IMembershipContext MembershipContext { get; }
        IUserContext UserContext { get; }
    }
}