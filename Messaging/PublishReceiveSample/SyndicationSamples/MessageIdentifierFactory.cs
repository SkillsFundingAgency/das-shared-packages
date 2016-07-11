using System;
using SFA.DAS.Messaging.Syndication;

namespace PublishReceiveSample.SyndicationSamples
{
    class MessageIdentifierFactory : IMessageIdentifierFactory
    {
        public IMessageIdentifier<T> Create<T>()
        {
            if (typeof(T) == typeof(SampleEvent))
            {
                return (IMessageIdentifier<T>)new SampleEventMessageIdentifier();
            }
            throw new ArgumentException("No identifier registered for type " + typeof(T).Name);
        }
    }
}
