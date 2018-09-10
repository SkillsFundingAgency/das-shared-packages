using NServiceBus;

namespace SFA.DAS.NServiceBus.NewtonsoftJsonSerializer
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseNewtonsoftJsonSerializer(this EndpointConfiguration config)
        {
            config.UseSerialization<NewtonsoftSerializer>();

            return config;
        }
    }
}