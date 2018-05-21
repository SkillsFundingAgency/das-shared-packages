namespace SFA.DAS.Messaging.Interfaces
{
    /// <summary>
    ///     A service that allows a message processor to obtain information about the underlying message (rather than just its content).
    /// </summary>
    public interface IMessageContextProvider
    {
        void StoreMessageContext<TMessageContentType>(IMessage<TMessageContentType> message) where TMessageContentType : class;
        MessageContext GetContextForMessageBody(object messageContent);
        void ReleaseMessageContext<TMessageContentType>(IMessage<TMessageContentType> message) where TMessageContentType : class;
    }
}