using System.Data.Entity;
using System.Web.Http.Filters;
using System.Web.Mvc;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupEntityFrameworkUnitOfWork<T>(this EndpointConfiguration config) where T : DbContext, IOutboxDbContext
        {
            config.RegisterComponents(c =>
            {
                c.ConfigureComponent<Db<T>>(DependencyLifecycle.InstancePerUnitOfWork);
                c.ConfigureComponent<Outbox<T>>(DependencyLifecycle.InstancePerUnitOfWork);
            });

            config.SetupUnitOfWork();

            return config;
        }

        public static EndpointConfiguration SetupEntityFrameworkUnitOfWork<T>(this EndpointConfiguration config, GlobalFilterCollection filters) where T : DbContext, IOutboxDbContext
        {
            config.SetupEntityFrameworkUnitOfWork<T>();
            filters.Add(new Mvc.UnitOfWorkManagerFilter(() => DependencyResolver.Current.GetService<IUnitOfWorkManager>()), -999);

            return config;
        }

        public static EndpointConfiguration SetupEntityFrameworkUnitOfWork<T>(this EndpointConfiguration config, HttpFilterCollection filters) where T : DbContext, IOutboxDbContext
        {
            config.SetupEntityFrameworkUnitOfWork<T>();
            filters.Add(new WebApi.UnitOfWorkManagerFilter());

            return config;
        }
    }
}