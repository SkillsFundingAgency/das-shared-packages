namespace SFA.DAS.Authorization
{
    public interface IAccountMessage
    {
        string AccountHashedId { get; set; }
        long? AccountId { get; set; }
    }
}