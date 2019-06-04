namespace SFA.DAS.Authorization
{
    public interface IAccountContext
    {
        long Id { get; }
        string HashedId { get; }
        string PublicHashedId { get; }
    }
}