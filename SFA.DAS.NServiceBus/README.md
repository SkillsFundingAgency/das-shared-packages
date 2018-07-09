# SFA.DAS.NServiceBus

This package includes:

* Extension methods for `NServiceBus.EndpointConfiguration` that can be used to ensure NServiceBus is configured consistenly across `SFA.DAS` modules.
* Unit of work behavior to support data persistence and event publishing without polluting your code with `_repository.Save()` calls.
* Infrastructure to support transactional data persistence and event publishing from MVC and WebApi clients.

## Configuration

### NServiceBus Endpoint

The following example configures an NServiceBus endpoint to allow both sending and receiving of messages. By enabling NServiceBus' outbox feature the endpoint will simulate the reliability of a distributed transaction, guaranteeing consistency between data persistence and messaging operations:

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

### MVC Endpoint

The following example configures an NServiceBus endpoint within an MVC application to allow sending and receiving of messages. Unfortunately NServiceBus' outbox feature only runs in the context of processing an incoming message and not an HTTP request. However, by passing MVC's `GlobalFilters.Filters` collection to the `SetupOutbox` method the package will replicate NServiceBus' outbox feature when publishing messages on the client:

```c#
var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Web")
    .SetupAzureServiceBusTransport(() => container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().MessageServiceBusConnectionString)
    .SetupEntityFrameworkUnitOfWork<EmployerAccountsDbContext>()
    .SetupErrorQueue()
    .SetupInstallers()
    .SetupMsSqlServerPersistence(() => container.GetInstance<DbConnection>())
    .SetupNewtonsoftSerializer()
    .SetupNLogFactory()
    .SetupOutbox(GlobalFilters.Filters)
    .SetupStructureMapBuilder(container);
```

### WebApi Endpoint

The following example configures an NServiceBus endpoint within a WebApi application to allow sending and receiving of messages. Unfortunately NServiceBus' outbox feature only runs in the context of processing an incoming message and not an HTTP request. However, by passing WebApi's `GlobalConfiguration.Configuration.Filters` collection to the `SetupOutbox` method the package will replicate NServiceBus' outbox feature when publishing messages on the client:

```c#
var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Api")
    .SetupAzureServiceBusTransport(() => container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().MessageServiceBusConnectionString)
    .SetupEntityFrameworkUnitOfWork<EmployerAccountsDbContext>()
    .SetupErrorQueue()
    .SetupInstallers()
    .SetupMsSqlServerPersistence(() => container.GetInstance<DbConnection>())
    .SetupNewtonsoftSerializer()
    .SetupNLogFactory()
    .SetupOutbox(GlobalConfiguration.Configuration.Filters)
    .SetupStructureMapBuilder(container);
```

### Job

In the case of a crash or a service outage between an MVC or WebApi client and the server, some messages will fail to be published immediately. To accomodate these scenarios the `IProcessOutboxMessagesJob` should be run periodically. Here's an example of triggering the job to run every 24 hours using an Azure function:

```c#
public static Task ProcessOutboxMessages([TimerTrigger("0 0 0 * * *")] TimerInfo timer, TraceWriter logger)
{
    var job = ServiceLocator.GetInstance<IProcessOutboxMessagesJob>();
    return job.RunAsync();
}
```

### MS SQL Server

When configuring an MVC or WebApi client as above the following tables will need to be created as part of a deployment:

```sql
CREATE TABLE [dbo].[OutboxData]
(
    [MessageId] NVARCHAR(200) NOT NULL PRIMARY KEY NONCLUSTERED,
    [Dispatched] BIT NOT NULL DEFAULT(0),
    [DispatchedAt] DATETIME NULL,
    [PersistenceVersion] VARCHAR(23) NOT NULL,
    [Operations] NVARCHAR(MAX) NOT NULL,
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
    [Created] DATETIME NOT NULL,
    [Dispatched] BIT NOT NULL DEFAULT(0),
    [DispatchedAt] DATETIME NULL,
    [Operations] NVARCHAR(MAX) NOT NULL
)
GO

CREATE INDEX [IX_Created] ON [dbo].[ClientOutboxData] ([Created] ASC) WHERE [Dispatched] = 0
GO

CREATE INDEX [IX_DispatchedAt] ON [dbo].[ClientOutboxData] ([DispatchedAt] ASC) WHERE [Dispatched] = 1
GO
```

## Transactions

### Saving Data

To ensure data is saved and messages are published as part of the same transaction then any database operations will need to be included in the same unit of work. By taking a dependency on `SFA.DAS.NServiceBus.IUnitOfWorkContext` this will give you access to the current request's connection and transaction, for example if you're using an SQL database:

```c#
var connection = _unitofWorkContext.Get<DbConnection>();
var transaction = _unitofWorkContext.Get<DbTransaction>();
```

### Publishing Messages

To ensure data is saved and messages are published as part of the same transaction then any messaging operations will need to be included in the same unit of work. By taking a dependency on `SFA.DAS.NServiceBus.IUnitOfWorkContext` this will give you access to the current request's events collection, for example:

```c#
_unitOfWorkContext.AddEvent(new SomethingHappenedEvent());
```

There's also a static version of the above method available that can be used when you're not running in the context of an IoC container, for example in your domain models:

```c#
UnitOfWorkContext.AddEvent(new SomethingHappenedEvent());
```

### Legacy SFA.DAS.Messaging

If you're currently using the `SFA.DAS.Messaging.IMessagePublisher` interface to publish messages on the client but would like to use this package instead then the equivalent interface is `SFA.DAS.NServiceBus.IEventPublisher` which exposes a `Task Publish<T>(T message)` method.

## Extension

If you're not using MS SQL Server then the infrastructure can be extended easily to accomodate your stack. For example, if you're using RavenDB then you'll just need to add a custom implementation of `IOutbox`. `IOutbox`'s signature currently looks like this.

```c#
public interface IOutbox
{
    Task<IOutboxTransaction> BeginTransactionAsync();
    Task<OutboxMessage> GetAsync(Guid messageId);
    Task<IEnumerable<IOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync();
    Task SetAsDispatchedAsync(Guid messageId);
    Task StoreAsync(OutboxMessage outboxMessage, IOutboxTransaction outboxTransaction);
}
```
