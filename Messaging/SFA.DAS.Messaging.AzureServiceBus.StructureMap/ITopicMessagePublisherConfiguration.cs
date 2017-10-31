namespace SFA.DAS.Messaging.AzureServiceBus.StructureMap
{
    public interface ITopicMessagePublisherConfiguration
    {
        string MessageServiceBusConnectionString { get; }
    }
}
