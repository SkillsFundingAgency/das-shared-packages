namespace SFA.DAS.Messaging.Interfaces
{
    public interface IMessageSubscriberFactory 
    {
        IMessageSubscriber<T> GetSubscriber<T>() where T : new();
    }
}
