using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.Data;
using SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.StartupTasks;
using SFA.DAS.NServiceBus.Utilities;

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
            context.Container.ConfigureComponent(b => new AsyncTimer(Task.Delay), DependencyLifecycle.InstancePerCall);
            context.Container.ConfigureComponent<ClientOutboxCleaner>(DependencyLifecycle.InstancePerCall);
            context.Container.ConfigureComponent<ClientOutboxPersister>(DependencyLifecycle.InstancePerCall);
            context.Container.ConfigureComponent<ClientOutboxPersisterV2>(DependencyLifecycle.InstancePerCall);
            
            if (!context.Settings.GetOrDefault<bool>("Persistence.Sql.Outbox.DisableCleanup"))
            {
                context.RegisterStartupTask(b => b.Build<ClientOutboxCleaner>());
            }
        }
    }
}