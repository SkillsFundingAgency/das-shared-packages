using NServiceBus;

namespace SFA.DAS.NServiceBus.Extensions;

public static class TransportExtensions
{
    public static void AddRouting(this TransportExtensions<AzureServiceBusTransport> transport, Action<RoutingSettings<AzureServiceBusTransport>> routesToAdd)
    {
        var settings = transport.Routing();
        routesToAdd(settings);
    }
}