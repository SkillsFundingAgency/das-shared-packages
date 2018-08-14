#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

namespace SFA.DAS.NServiceBus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNServiceBus(this IServiceCollection services, EndpointConfiguration endpointConfiguration)
        {
            var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            services.AddSingleton(endpoint);
            services.AddSingleton<IMessageSession>(endpoint);
            return services;
        }
    }
}
#endif
