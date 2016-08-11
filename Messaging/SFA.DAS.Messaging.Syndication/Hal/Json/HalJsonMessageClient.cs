using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Messaging.Syndication.Http;

namespace SFA.DAS.Messaging.Syndication.Hal.Json
{
    public class HalJsonMessageClient : IMessageClient
    {
        private readonly IFeedPositionRepository _feedPositionRepository;
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly IMessageIdentifierFactory _messageIdentifierFactory;

        public HalJsonMessageClient(IFeedPositionRepository feedPositionRepository, IHttpClientWrapper httpClientWrapper, IMessageIdentifierFactory messageIdentifierFactory)
        {
            _feedPositionRepository = feedPositionRepository;
            _httpClientWrapper = httpClientWrapper;
            _messageIdentifierFactory = messageIdentifierFactory;
        }

        public async Task<ClientMessage<T>> GetNextUnseenMessage<T>()
        {
            var lastMessageId = await _feedPositionRepository.GetLastSeenMessageIdentifierAsync();
            var messageIdentifier = _messageIdentifierFactory.Create<T>();

            var page = await GetPage<T>("/");
            if (page == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(lastMessageId))
            {
                var message = page.Embedded.Messages.FirstOrDefault();
                return message != null ? ConvertMessageToClientMessages<T>(message, messageIdentifier) : null;
            }

            var messages = new ClientMessage<T>[0];
            var indexOfLastMessage = -1;
            while (indexOfLastMessage == -1)
            {
                messages = page.Embedded.Messages.Select(m => ConvertMessageToClientMessages(m, messageIdentifier)).ToArray();
                indexOfLastMessage = IndexOf(messages, lastMessageId);

                // Was it not on this page
                if (indexOfLastMessage == -1)
                {
                    // we have run out of pages, so return the first
                    if (string.IsNullOrEmpty(page.Links.Next))
                    {
                        page = await GetPage<T>(page.Links.First);
                        messages = page.Embedded.Messages.Select(m => ConvertMessageToClientMessages(m, messageIdentifier)).ToArray();
                        return messages[0];
                    }

                    page = await GetPage<T>(page.Links.Next);
                    continue;
                }

                // Did we find a message, but is the last one on the page
                if (indexOfLastMessage >= messages.Length - 1)
                {
                    if (string.IsNullOrEmpty(page.Links.Next))
                    {
                        return null;
                    }

                    page = await GetPage<T>(page.Links.Next);
                    messages = page.Embedded.Messages.Select(m => ConvertMessageToClientMessages(m, messageIdentifier)).ToArray();
                    return messages[0];
                }
            }

            return messages[indexOfLastMessage + 1];
        }

        private async Task<HalPage<T>> GetPage<T>(string pageUrl)
        {
            var response = await _httpClientWrapper.Get(pageUrl);
            if (response == null)
            {
                return null;
            }
            var page = JsonConvert.DeserializeObject<HalPage<T>>(response);
            return page;
        }
        private ClientMessage<T> ConvertMessageToClientMessages<T>(T message, IMessageIdentifier<T> messageIdentifier)
        {
            var identifier = messageIdentifier.GetIdentifier(message);
            return new ClientMessage<T>
            {
                Identifier = identifier,
                Message = message
            };
        }
        private int IndexOf<T>(ClientMessage<T>[] messages, string identifier)
        {
            for (var i = 0; i < messages.Length; i++)
            {
                if (messages[i].Identifier.Equals(identifier))
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
