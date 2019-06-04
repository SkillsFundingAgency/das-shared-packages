namespace SFA.DAS.Authorization.Mvc
{
    public interface IAccountViewModel
    {
        long AccountId { get; set; }
        string AccountHashedId { get; set; }
    }
}