# SFA.DAS.NServiceBus

This package includes:

* Extension methods for `NServiceBus.EndpointConfiguration` that can be used to ensure NServiceBus is configured consistenly across `SFA.DAS` modules.
* Infrastructure to support transactional data persistence and event publishing from MVC and WebApi clients.

## Configuration

### NServiceBus Send & Receive Endpoint

The following example configures an NServiceBus endpoint to allow both sending and receiving of messages. By enabling NServiceBus' outbox feature the endpoint will simulate the reliability of a distributed transaction, guaranteeing consistency between data persistence and messaging operations.

```c#
var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.MessageHandlers")
    .SetupAzureServiceBusTransport(() => container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().MessageServiceBusConnectionString)
    .SetupEntityFrameworkUnitOfWork<EmployerAccountsDbContext>()
    .SetupErrorQueue()
    .SetupInstallers()
    .SetupMsSqlServerPersistence(() => container.GetInstance<DbConnection>())
    .SetupNewtonsoftSerializer()
    .SetupNLogFactory()
    .SetupOutbox()
    .SetupStructureMapBuilder(container);
```

### MVC Send Only Endpoint

The following example configures an NServiceBus endpoint within an MVC application to allow sending of messages only. Unfortunately NServiceBus' outbox feature only runs in the context of processing an incoming message. However, by passing MVC's `GlobalFilters.Filters` collection to the `SetupEntityFrameworkUnitOfWork` method the infrastructure replicates NServiceBus' outbox feature when publishing messages on the client.

```c#
var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Web")
    .SetupAzureServiceBusTransport(() => container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().MessageServiceBusConnectionString)
    .SetupEntityFrameworkUnitOfWork<EmployerAccountsDbContext>(GlobalFilters.Filters)
    .SetupErrorQueue()
    .SetupInstallers()
    .SetupNewtonsoftSerializer()
    .SetupNLogFactory()
    .SetupSendOnly()
    .SetupStructureMapBuilder(container);
```

### WebApi Send Only Endpoint

The following example configures an NServiceBus endpoint within a WebApi application to allow sending of messages only. Unfortunately NServiceBus' outbox feature only runs in the context of processing an incoming message. However, by passing WebApi's `GlobalConfiguration.Configuration.Filters` collection to the `SetupEntityFrameworkUnitOfWork` method the infrastructure replicates NServiceBus' outbox feature when publishing messages on the client.

```c#
var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Api")
    .SetupAzureServiceBusTransport(() => container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().MessageServiceBusConnectionString)
    .SetupEntityFrameworkUnitOfWork<EmployerAccountsDbContext>(GlobalConfiguration.Configuration.Filters)
    .SetupErrorQueue()
    .SetupInstallers()
    .SetupNewtonsoftSerializer()
    .SetupNLogFactory()
    .SetupSendOnly()
    .SetupStructureMapBuilder(container);
```

### Job

In the case of a crash or a service outage between an MVC or WebAPI client and the server, some messages will fail to be published immediately. To accomodate these scenarios the `IProcessOutboxMessagesJob` should be run periodically. Here's an example of triggering the job to run every 24 hours using an Azure function.

```c#
public static Task ProcessOutboxMessages([TimerTrigger("0 0 0 * * *")] TimerInfo timer, TraceWriter logger)
{
    var job = ServiceLocator.GetInstance<IProcessOutboxMessagesJob>();
    return job.RunAsync();
}
```

### MS SQL Server

When configuring an MVC or WebAPi client as above the following table will need to be created as part of a deployment.

```sql
CREATE TABLE [dbo].[OutboxData]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Sent] DATETIME NOT NULL,
	[Published] DATETIME NULL,
	[Data] NVARCHAR(MAX) NOT NULL
)
GO

CREATE INDEX [IX_OutboxData] ON [dbo].[OutboxData] ([Sent] ASC, [Published] ASC)
GO
```

## Extension

If you're not using the above database or ORM technologies then the infrastructure can be extended easily to accomodate your stack. For example, if you're using Dapper instead of Entity Framework then you'll just need to add a custom implementation of `IOutbox` along with a `SetupDapperUnitOfWork` extension method to register it with the container. `IOutbox`'s signature currently looks like this:

```c#
public interface IOutbox
{
    Task<IOutboxTransaction> BeginTransactionAsync();
    Task AddAsync(OutboxMessage outboxMessage);
    Task<OutboxMessage> GetById(Guid id);
    Task<IEnumerable<Guid>> GetIdsToProcess();
}
```

The Entity Framework implementation of `IOutbox` can be found here as an example: https://github.com/SkillsFundingAgency/das-shared-packages/tree/master/SFA.DAS.NServiceBus/SFA.DAS.NServiceBus/EntityFramework/Outbox.cs
