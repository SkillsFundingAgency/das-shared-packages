using System.Web.Http.Filters;
using NServiceBus;

namespace SFA.DAS.NServiceBus.WebApi
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupOutbox(this EndpointConfiguration config, HttpFilterCollection filters)
        {
            config.SetupOutbox();
            filters.Add(new UnitOfWorkManagerFilter());

            return config;
        }
    }
}