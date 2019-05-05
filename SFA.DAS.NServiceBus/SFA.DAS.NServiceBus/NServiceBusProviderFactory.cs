using System;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.ObjectBuilder.MSDependencyInjection;

namespace SFA.DAS.NServiceBus
{
    public class NServiceBusProviderFactory : IServiceProviderFactory<UpdateableServiceProvider>
    {
        public UpdateableServiceProvider CreateBuilder(IServiceCollection services)
        {
            return new UpdateableServiceProvider(services);
        }

        public IServiceProvider CreateServiceProvider(UpdateableServiceProvider containerBuilder)
        {
            return containerBuilder;
        }
    }
}