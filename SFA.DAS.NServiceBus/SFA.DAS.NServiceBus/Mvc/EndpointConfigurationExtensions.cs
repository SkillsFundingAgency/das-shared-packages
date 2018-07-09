using System.Web.Mvc;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Mvc
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupOutbox(this EndpointConfiguration config, GlobalFilterCollection filters)
        {
            config.SetupOutbox();
            filters.Add(new UnitOfWorkManagerFilter(() => DependencyResolver.Current.GetService<IUnitOfWorkManager>()), -999);

            return config;
        }
    }
}