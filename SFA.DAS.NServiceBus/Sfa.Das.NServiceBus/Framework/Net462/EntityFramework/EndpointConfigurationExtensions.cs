using System.Data.Entity;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupEntityFrameworkUnitOfWork<T>(this EndpointConfiguration config) where T : DbContext
        {
            config.RegisterComponents(c =>
            {
                c.ConfigureComponent<Db<T>>(DependencyLifecycle.InstancePerUnitOfWork);
            });

            config.SetupUnitOfWork();

            return config;
        }
    }
}