using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.Data;
using SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.StartupTasks;

namespace SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox
{
    public class SqlServerClientOutboxFeature : Feature
    {
        public SqlServerClientOutboxFeature()
        {
            DependsOn("NServiceBus.Persistence.Sql.SqlOutboxFeature");
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Container.ConfigureComponent<ClientOutboxCleaner>(DependencyLifecycle.InstancePerCall);
            context.Container.ConfigureComponent<ClientOutboxPersister>(DependencyLifecycle.InstancePerCall);
            context.Container.ConfigureComponent<ClientOutboxPersisterV2>(DependencyLifecycle.InstancePerCall);
            context.Container.ConfigureComponent<DateTimeService>(DependencyLifecycle.InstancePerCall);
            context.Container.ConfigureComponent(b => new TimerService(b.Build<IDateTimeService>(), Task.Delay), DependencyLifecycle.InstancePerCall);
            
            if (!context.Settings.GetOrDefault<bool>("Persistence.Sql.Outbox.DisableCleanup"))
            {
                context.RegisterStartupTask(b => b.Build<ClientOutboxCleaner>());
            }
        }
    }
}