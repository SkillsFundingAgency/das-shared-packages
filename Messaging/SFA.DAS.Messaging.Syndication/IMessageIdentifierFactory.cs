namespace SFA.DAS.Messaging.Syndication
{
    public interface IMessageIdentifierFactory
    {
        IMessageIdentifier<T> Create<T>();
    }
}