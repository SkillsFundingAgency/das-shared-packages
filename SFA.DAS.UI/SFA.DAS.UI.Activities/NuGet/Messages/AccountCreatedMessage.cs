

namespace NuGet.Messages
{
    public class AccountCreatedMessage : Message
    {
        public AccountCreatedMessage()
        {
            
        }

        public AccountCreatedMessage(long accountId)
        {
            AccountId = accountId;
        }
        public long AccountId { get; }
    }
}
