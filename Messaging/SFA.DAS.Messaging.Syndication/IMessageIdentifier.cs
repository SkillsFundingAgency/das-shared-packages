namespace SFA.DAS.Messaging.Syndication
{
    public interface IMessageIdentifier<T>
    {
        string GetIdentifier(T message);
    }
}
