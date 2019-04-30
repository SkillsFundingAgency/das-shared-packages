using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.UnitOfWork.NServiceBus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNServiceBusUnitOfWork(this IServiceCollection services)
        {
            return services.AddUnitOfWork()
                .AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}