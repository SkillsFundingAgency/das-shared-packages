using Newtonsoft.Json;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Extensions;

public static class EndpointConfigurationExtensions
{
    public static EndpointConfiguration UseNewtonsoftJsonSerializer(
        this EndpointConfiguration config)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None
        };

        var serialization = config.UseSerialization<NewtonsoftJsonSerializer>();
        serialization.Settings(settings);
        return config;
    }

    public static EndpointConfiguration UseMessageConventions(this EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration.Conventions()
            .DefiningMessagesAs(IsMessage)
            .DefiningEventsAs(IsEvent)
            .DefiningCommandsAs(IsCommand);

        return endpointConfiguration;
    }

    public static bool IsMessage(Type t) => IsSfaMessage(t, "Messages");

    public static bool IsEvent(Type t) => IsSfaMessage(t, "Messages.Events");

    public static bool IsCommand(Type t) => IsSfaMessage(t, "Messages.Commands");

    public static bool IsSfaMessage(Type t, string namespaceSuffix)
        => t.Namespace != null &&
               t.Namespace.StartsWith("SFA.DAS") &&
               t.Namespace.EndsWith(namespaceSuffix);
    
}