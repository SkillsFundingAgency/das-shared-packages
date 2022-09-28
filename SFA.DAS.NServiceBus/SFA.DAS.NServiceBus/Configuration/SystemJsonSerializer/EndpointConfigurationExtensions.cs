using NServiceBus;

namespace SFA.DAS.NServiceBus.Configuration.SystemJsonSerializer
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseSystemJsonSerializer(this EndpointConfiguration config)
        {
            config.UseSerialization<global::NServiceBus.Json.SystemJsonSerializer>();

            return config;
        }
    }
}