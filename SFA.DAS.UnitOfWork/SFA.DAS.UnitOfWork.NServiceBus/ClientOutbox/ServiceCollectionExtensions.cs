using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNServiceBusClientUnitOfWork(this IServiceCollection services)
        {
            services.TryAddScoped<IEventPublisher, EventPublisher>();

            return services.AddUnitOfWork()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();
        }
    }
}