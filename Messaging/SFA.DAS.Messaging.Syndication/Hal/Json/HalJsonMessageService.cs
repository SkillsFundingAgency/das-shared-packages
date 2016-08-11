using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SFA.DAS.Messaging.Syndication.Hal.Json
{
    public class HalJsonMessageService<T>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IHalResourceAttributeExtrator<T> _attributeExtractor;
        private readonly IHalPageLinkBuilder _pageLinkBuilder;

        public HalJsonMessageService(IMessageRepository messageRepository, IHalResourceAttributeExtrator<T> attributeExtractor, IHalPageLinkBuilder pageLinkBuilder)
        {
            _messageRepository = messageRepository;
            _attributeExtractor = attributeExtractor;
            _pageLinkBuilder = pageLinkBuilder;
        }

        public async Task<HalResponse> GetPageAsync(int page)
        {
            var rawPage = await _messageRepository.RetreivePageAsync<T>(page);
            var hasMorePages = page < rawPage.TotalNumberOfPages;

            var halPage = new HalPage<dynamic>
            {
                Links = new HalPageLinks
                {
                    Next = hasMorePages ? _pageLinkBuilder.NextPage(page + 1) : null,
                    Prev = page > 1 ? _pageLinkBuilder.PreviousPage(page - 1) : null,
                    First = rawPage.TotalNumberOfMessages > 0 ? _pageLinkBuilder.FirstPage(1) : null,
                    Last = rawPage.TotalNumberOfMessages > 0 ? _pageLinkBuilder.LastPage(rawPage.TotalNumberOfPages) : null
                },
                Count = rawPage.TotalNumberOfMessages,
                Embedded = new HalContent<dynamic>
                {
                    Messages = rawPage.Messages.Select(msg => BuildEmbeddedMessage(_attributeExtractor.Extract(msg))).ToArray()
                }
            };

            return new HalResponse
            {
                Headers = new Dictionary<string, string[]>
                {
                    { "Content-Type", new[] { "application/hal+json" } }
                },
                Content = JsonConvert.SerializeObject(halPage, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                })
            };
        }

        private dynamic BuildEmbeddedMessage(HalResourceAttributes attributes)
        {
            var resource = new ExpandoObject();
            var resourceProperties = (ICollection<KeyValuePair<string, object>>)resource;
            foreach (var keyValuePair in attributes.Properties)
            {
                resourceProperties.Add(new KeyValuePair<string, object>(keyValuePair.Key, keyValuePair.Value));
            }

            var links = new ExpandoObject();
            var linksProperties = (ICollection<KeyValuePair<string, object>>)links;
            foreach (var keyValuePair in attributes.Links)
            {
                linksProperties.Add(new KeyValuePair<string, object>(keyValuePair.Key, keyValuePair.Value));
            }
            resourceProperties.Add(new KeyValuePair<string, object>("_links", linksProperties));

            return resource;
        }
    }
}
