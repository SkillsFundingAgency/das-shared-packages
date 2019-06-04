namespace SFA.DAS.Authorization
{
    public class AccountContext : IAccountContext
    {
        public long Id { get; set; }
        public string HashedId { get; set; }
        public string PublicHashedId { get; set; }
    }
}