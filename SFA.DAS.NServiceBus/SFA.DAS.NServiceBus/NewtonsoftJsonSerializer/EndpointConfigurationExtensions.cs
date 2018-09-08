using NServiceBus;

namespace SFA.DAS.NServiceBus.NewtonsoftJsonSerializer
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupNewtonsoftJsonSerializer(this EndpointConfiguration config)
        {
            config.UseSerialization<NewtonsoftSerializer>();

            return config;
        }
    }
}