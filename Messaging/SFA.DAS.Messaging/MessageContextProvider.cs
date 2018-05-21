using System.Collections.Concurrent;
using SFA.DAS.Messaging.Exceptions;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging
{
    public class MessageContextProvider : IMessageContextProvider
    {
        private readonly ConcurrentDictionary<object, MessageContext> _messageContexts = new ConcurrentDictionary<object, MessageContext>();

        public void StoreMessageContext<TMessageContentType>(IMessage<TMessageContentType> message)  where TMessageContentType : class
        {
            if (message?.Content == null)
            {
                throw new MessageContextException(typeof(TMessageContentType), "The message is null or has no content");
            }

            _messageContexts.GetOrAdd(message.Content, id => new MessageContext {MessageId = message.Id, MessageContent = message.Content});
        }

        public MessageContext GetContextForMessageBody(object messageContent)
        {
            if (messageContent == null)
            {
                throw new MessageContextException("Cannot get message context for null message content");
            }

            _messageContexts.TryGetValue(messageContent, out MessageContext messageContext);
            return messageContext;
        }

        public void ReleaseMessageContext<TMessageContentType>(IMessage<TMessageContentType> message) where TMessageContentType : class
        {
            if (message?.Content != null)
            {
                _messageContexts.TryRemove(message.Content, out MessageContext storedMessageContext);

                if (storedMessageContext == null)
                {
                    throw new MessageContextException(typeof(TMessageContentType), "The message cannot be released because it has not previously been stored or has already been released.");
                }
            }
        }
    }
}