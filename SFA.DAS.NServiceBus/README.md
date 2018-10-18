# SFA.DAS.NServiceBus

This package includes:

* Infrastructure to support transactional data persistence and event publishing on the client.
* Extension methods for `NServiceBus.EndpointConfiguration` to wire up common behaviour.

## Configuration

### NServiceBus Endpoint

The following example configures an NServiceBus endpoint to allow both sending and receiving of messages. By enabling NServiceBus' outbox feature the endpoint will simulate the reliability of a distributed transaction, guaranteeing consistency between data persistence and messaging operations:

```c#
var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.MessageHandlers")
    .UseAzureServiceBusTransport(false, () => container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().MessageServiceBusConnectionString, r => {})
    .UseErrorQueue()
    .UseInstallers()
    .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
    .UseNewtonsoftJsonSerializer()
    .UseNLogFactory()
    .UseOutbox()
    .UseStructureMapBuilder(container)
    .UseUnitOfWork();
```

### MVC Endpoint

The following example configures an NServiceBus endpoint within an MVC application to allow sending and receiving of messages. Unfortunately NServiceBus' outbox feature only runs in the context of processing an incoming message and not an HTTP request. However, by registering a `IUnitOfWorkManager` in the container from the `SFA.DAS.UnitOfWork.NServiceBus` package and calling the `AddUnitOfWorkFilter()` extension method on the `GlobalFilters.Filters` collection then NServiceBus' outbox feature will be replicated when publishing messages on the client:

```c#
var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Web")
    .UseAzureServiceBusTransport(false, () => container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().MessageServiceBusConnectionString, r => {})
    .UseErrorQueue()
    .UseInstallers()
    .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
    .UseNewtonsoftJsonSerializer()
    .UseNLogFactory()
    .UseOutbox()
    .UseStructureMapBuilder(container)
    .UseUnitOfWork();

filters.AddUnitOfWorkFilter();
```

### MVC Core Endpoint

The following example configures an NServiceBus endpoint within an MVC Core application to allow sending and receiving of messages. Unfortunately NServiceBus' outbox feature only runs in the context of processing an incoming message and not an HTTP request. However, by registering a `IUnitOfWorkManager` in the container from the `SFA.DAS.UnitOfWork.NServiceBus` package and calling the `UseUnitOfWork()` extension method on `IApplicationBuilder` then NServiceBus' outbox feature will be replicated when publishing messages on the client:

```c#
var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Web")
    .UseAzureServiceBusTransport(false, () => container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().MessageServiceBusConnectionString, r => {})
    .UseErrorQueue()
    .UseInstallers()
    .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
    .UseNewtonsoftJsonSerializer()
    .UseNLogFactory()
    .UseOutbox()
    .UseStructureMapBuilder(container)
    .UseUnitOfWork();

app.UseUnitOfWork();
```

### WebApi Endpoint

The following example configures an NServiceBus endpoint within a WebApi application to allow sending and receiving of messages. Unfortunately NServiceBus' outbox feature only runs in the context of processing an incoming message and not an HTTP request. However, by registering a `IUnitOfWorkManager` in the container from the `SFA.DAS.UnitOfWork.NServiceBus` package and calling the `AddUnitOfWorkFilter()` extension method on the `GlobalConfiguration.Configuration.Filters` collection then NServiceBus' outbox feature will be replicated when publishing messages on the client:

```c#
var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Api")
    .UseAzureServiceBusTransport(false, () => container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().MessageServiceBusConnectionString, r => {})
    .UseErrorQueue()
    .UseInstallers()
    .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
    .UseNewtonsoftJsonSerializer()
    .UseNLogFactory()
    .UseOutbox()
    .UseStructureMapBuilder(container)
    .UseUnitOfWork();

filters.AddUnitOfWorkFilter();
```

### Job

In the case of a crash or a service outage between an MVC or WebApi client and the server, some messages will fail to be published immediately. To accomodate these scenarios the `IProcessClientOutboxMessagesJob` should be run periodically. Here's an example of triggering the job to run every 24 hours using an Azure function:

```c#
public static Task ProcessClientOutboxMessages([TimerTrigger("0 0 0 * * *")] TimerInfo timer, TraceWriter logger)
{
    var job = ServiceLocator.GetInstance<IProcessClientOutboxMessagesJob>();
    return job.RunAsync();
}
```

### SQL Server

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
    [CreatedAt] DATETIME NOT NULL,
    [Dispatched] BIT NOT NULL DEFAULT(0),
    [DispatchedAt] DATETIME NULL,
    [Operations] NVARCHAR(MAX) NOT NULL
)
GO

CREATE INDEX [IX_CreatedAt] ON [dbo].[ClientOutboxData] ([CreatedAt] ASC) WHERE [Dispatched] = 0
GO

CREATE INDEX [IX_DispatchedAt] ON [dbo].[ClientOutboxData] ([DispatchedAt] ASC) WHERE [Dispatched] = 1
GO
```

## Transactions

### Saving Data

To ensure data is saved and messages are published as part of the same transaction then any database operations will need to be included in the same unit of work. By taking a dependency on `SFA.DAS.NServiceBus.IUnitOfWorkContext` this will give you access to the current request's persistence session, for example if you're using an SQL database:

### Client

```c#
var session = _unitOfWorkContext.Get<IClientOutboxTransaction>();
var sqlSession = session.GetSqlSession();
var connection = sqlSession.Connection;
var transaction = sqlSession.Transaction;
```

### Server

```c#
var session = _unitOfWorkContext.Get<SynchronizedStorageSession>();
var sqlSession = session.GetSqlSession();
var connection = sqlSession.Connection;
var transaction = sqlSession.Transaction;
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

If you're currently using the `SFA.DAS.Messaging.IMessagePublisher` interface to publish messages on the client but would like to use this package instead then the equivalent interface is `SFA.DAS.NServiceBus.IEventPublisher` which exposes a `Task Publish(object message)` method.

## Extension

If you're not using SQL Server then the infrastructure can be extended easily to accomodate your stack. For example, if you're using RavenDB then you'll just need to add a custom implementation of `IClientOutboxStorage`. `IClientOutboxStorage`'s signature currently looks like this:

```c#
public interface IClientOutboxStorage
{
    Task<IClientOutboxTransaction> BeginTransactionAsync();
    Task<ClientOutboxMessage> GetAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession);
    Task<IEnumerable<IClientOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync();
    Task SetAsDispatchedAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession);
    Task StoreAsync(ClientOutboxMessage clientOutboxMessage, IClientOutboxTransaction clientOutboxTransaction);
}
```
