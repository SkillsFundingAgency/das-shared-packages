namespace SFA.DAS.Messaging.Interfaces
{
    public interface IMessageSubscriberFactory<T> where T : new()
    {
        IMessageSubscriber<T> GetSubscriber();
    }
}
