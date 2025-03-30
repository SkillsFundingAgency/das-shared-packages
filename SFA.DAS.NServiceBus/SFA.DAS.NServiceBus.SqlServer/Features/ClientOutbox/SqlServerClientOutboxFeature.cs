using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.ObjectBuilder;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.Data;
using SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.StartupTasks;

namespace SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox
{
    public class SqlServerClientOutboxFeature : Feature
    {
        public SqlServerClientOutboxFeature()
        {
            DependsOn<Outbox>();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Services.AddTransient<ClientOutboxCleaner>();
            context.Services.AddTransient<ClientOutboxPersister>();
            context.Services.AddTransient<ClientOutboxPersisterV2>();
            context.Services.AddTransient<DateTimeService>();
            context.Services.AddTransient<TimerService>(provider =>
            {
                var dateTimeService = provider.GetRequiredService<IDateTimeService>();
                return new TimerService(dateTimeService, Task.Delay);
            });

            if (!context.Settings.GetOrDefault<bool>("Persistence.Sql.Outbox.DisableCleanup"))
            {
                context.RegisterStartupTask(b => b.GetService<ClientOutboxCleaner>());
            }
        }
    }
}