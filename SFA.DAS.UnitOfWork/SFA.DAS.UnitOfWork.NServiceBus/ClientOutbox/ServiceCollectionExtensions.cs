using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNServiceBusClientUnitOfWork(this IServiceCollection services)
        {
            return services.AddUnitOfWork()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();
        }
    }
}