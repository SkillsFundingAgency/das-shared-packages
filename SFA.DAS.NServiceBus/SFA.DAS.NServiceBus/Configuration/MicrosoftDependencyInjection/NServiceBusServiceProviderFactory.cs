using System;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection
{
    public class NServiceBusServiceProviderFactory
    {
        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }
    }
}
