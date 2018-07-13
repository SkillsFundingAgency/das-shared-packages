using NServiceBus;

namespace SFA.DAS.NServiceBus.NewtonsoftSerializer
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupNewtonsoftSerializer(this EndpointConfiguration config)
        {
            config.UseSerialization<global::NServiceBus.NewtonsoftSerializer>();

            return config;
        }
    }
}