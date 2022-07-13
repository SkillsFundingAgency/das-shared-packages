using NServiceBus;

namespace SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseNewtonsoftJsonSerializer(this EndpointConfiguration config)
        {
            config.UseSerialization<global::NServiceBus.NewtonsoftJsonSerializer>();

            return config;
        }
    }
}