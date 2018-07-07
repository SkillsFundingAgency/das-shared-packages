using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.NServiceBus
{
    public class OutboxMessage
    {
        public virtual Guid Id { get; protected set; }
        public virtual DateTime Sent { get; protected set; }
        public virtual DateTime? Published { get; protected set; }
        public virtual string Data { get; protected set; }

        public OutboxMessage(IEnumerable<Event> events)
        {
            Id = GuidComb.NewGuidComb();
            Sent = DateTime.UtcNow;
            Data = JsonConvert.SerializeObject(events, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        protected OutboxMessage()
        {
        }

        public IEnumerable<Event> Publish()
        {
            RequiresNotAlreadyPublished();

            Published = DateTime.UtcNow;

            return JsonConvert.DeserializeObject<List<Event>>(Data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        private void RequiresNotAlreadyPublished()
        {
            if (Published != null)
                throw new Exception("Requires not already published");
        }
    }
}