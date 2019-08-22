# SFA.DAS.NServiceBus

This package includes:

* Extension methods for `NServiceBus.EndpointConfiguration` to wire up common behaviour.
* Infrastructure to support NServiceBus' outbox feature on the client (e.g. an ASP.NET web application).
* Infrastructure to support integration between an NServiceBus endpoint and an Azure function.

## Extension Methods

The following example configures an NServiceBus endpoint to allow both sending and receiving of messages:

```c#
var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.MessageHandlers")
    .UseAzureServiceBusTransport(configuration.GetConnectionString("Bus"))
    .UseErrorQueue()
    .UseInstallers()
    .UseMessageConventions()
    .UseNewtonsoftJsonSerializer()
    .UseNLogFactory()
    .UseOutbox()
    .UseServicesBuilder(serviceProvider)
    .UseSqlServerPersistence(() => new SqlConnection(configuration.GetConnectionString("Db")));
```

## Outbox

By enabling NServiceBus' outbox feature the endpoint will simulate the reliability of a distributed transaction, guaranteeing consistency between data persistence and messaging operations.

### SQL Server

The following tables will need to be created before the endpoint is started:

```sql
CREATE TABLE [dbo].[OutboxData]
(
    [MessageId] NVARCHAR(200) NOT NULL PRIMARY KEY NONCLUSTERED,
    [Dispatched] BIT NOT NULL DEFAULT(0),
    [DispatchedAt] DATETIME NULL,
    [PersistenceVersion] VARCHAR(23) NOT NULL,
    [Operations] NVARCHAR(MAX) NOT NULL
)
GO

CREATE INDEX [IX_DispatchedAt] ON [dbo].[OutboxData] ([DispatchedAt] ASC) WHERE [Dispatched] = 1
GO
```

```sql
CREATE TABLE [dbo].[ClientOutboxData]
(
    [MessageId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
    [EndpointName] NVARCHAR(150) NOT NULL,
    [CreatedAt] DATETIME NOT NULL,
    [Dispatched] BIT NOT NULL DEFAULT(0),
    [DispatchedAt] DATETIME NULL,
    [PersistenceVersion] VARCHAR(23) NOT NULL,
    [Operations] NVARCHAR(MAX) NOT NULL
)
GO

CREATE INDEX [IX_CreatedAt_PersistenceVersion] ON [dbo].[ClientOutboxData] ([CreatedAt] ASC, [PersistenceVersion] ASC) WHERE [Dispatched] = 0
GO

CREATE INDEX [IX_DispatchedAt_PersistenceVersion] ON [dbo].[ClientOutboxData] ([DispatchedAt] ASC, [PersistenceVersion] ASC) WHERE [Dispatched] = 1
GO
``` 

### Cleanup

The number of outbox records will increase over time. Also, in the case of a service outage, some messages will not be dispatched immediately. To accomodate these scenarios outbox cleanup should be enabled, this will ensure that any stale records are deleted and that any messages awaiting dispatch are dispatched:

```c#
    .UseOutbox(enableCleanup: true)
```

It is advised to enable outbox cleanup on only one NServiceBus endpoint instance per database for the most efficient cleanup execution.

## Azure Function

To use NServiceBus in an Azure function you need to add the following to `Startup` and also have two environment variables, `NServiceBusConnectionString` and `NServiceBusLicense`:

```c#
public class Startup : IWebJobsStartup
{
    public void Configure(IWebJobsBuilder builder)
    {
        //Environment variables need creating for
        //      NServiceBusConnectionString
        //      NServiceBusLicense
        builder.AddExecutionContextBinding();
        builder.AddExtension<NServiceBusExtensionConfig>();
    }
}
```

Each Function endpoint then subscribes to a particular event as shown below:

```c#
public static async Task Run([NServiceBusTrigger(EndPoint = "SFA.DAS.NServiceBus.AzureFunctionExample")] NetFrameworkEvent message, ILogger log)
```

NetFrameworkEvent is the event that you wish to subscribe to, the EndPoint is then what is configured as the Subscription name and queue name. Each function endpoint should subscribe to one event.