using System;
using System.Collections.Generic;
using Moq;
using Newtonsoft.Json;

namespace SFA.DAS.NServiceBus.UnitTests
{
    public class OutboxMessageBuilder
    {
        private readonly Mock<OutboxMessage> _outboxMessage = new Mock<OutboxMessage>();

        public OutboxMessageBuilder WithId(Guid id)
        {
            _outboxMessage.Setup(m => m.Id).Returns(id);

            return this;
        }

        public OutboxMessageBuilder WithSent(DateTime sent)
        {
            _outboxMessage.Setup(m => m.Sent).Returns(sent);

            return this;
        }

        public OutboxMessageBuilder WithEvents(List<Event> events)
        {
            _outboxMessage.Setup(m => m.Data).Returns(JsonConvert.SerializeObject(events, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));

            return this;
        }

        public OutboxMessageBuilder WithPublished(DateTime? published)
        {
            _outboxMessage.Setup(m => m.Published).Returns(published);

            return this;
        }

        public OutboxMessage Build()
        {
            return _outboxMessage.Object;
        }
    }
}