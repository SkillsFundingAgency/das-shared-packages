using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.UnitOfWork.NServiceBus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNServiceBusUnitOfWork(this IServiceCollection services)
        {
            services.TryAddScoped<IEventPublisher, EventPublisher>();

            return services.AddUnitOfWork()
                .AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}