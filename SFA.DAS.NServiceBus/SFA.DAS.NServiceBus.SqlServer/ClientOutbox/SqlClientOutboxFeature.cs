using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.NServiceBus.SqlServer.ClientOutbox
{
    public class SqlClientOutboxFeature : Feature
    {
        public SqlClientOutboxFeature()
        {
            DependsOn<Outbox>();
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