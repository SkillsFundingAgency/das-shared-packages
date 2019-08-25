using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;

namespace SFA.DAS.NServiceBus.DependencyResolution.Microsoft
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseServicesBuilder(this EndpointConfiguration config, UpdateableServiceProvider serviceProvider)
        {
            config.UseContainer<ServicesBuilder>(c => c.ServiceProviderFactory(s => serviceProvider));

            return config;
        }
    }
}