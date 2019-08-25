using System;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.ObjectBuilder.MSDependencyInjection;

namespace SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection
{
    public class NServiceBusServiceProviderFactory : IServiceProviderFactory<UpdateableServiceProvider>
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