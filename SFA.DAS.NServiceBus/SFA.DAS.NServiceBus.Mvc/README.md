# SFA.DAS.NServiceBus.Mvc

This package includes:

* Extension methods for `IServiceCollection` that can be used to ensure NServiceBus is registered consistenly in the .net core DI Container.
* Unit of work behavior to support data persistence and event publishing without polluting your code with `_repository.Save()` calls.
* Infrastructure to support transactional data persistence and event publishing from MVC and WebApi clients.

## Configuration

### NServiceBus Endpoint

The following example configures an NServiceBus endpoint to allow both sending and receiving of messages. By enabling NServiceBus' outbox feature the endpoint will simulate the reliability of a distributed transaction, guaranteeing consistency between data persistence and messaging operations:

### .Net Standard 2.0

#### Add NserviceBus to ServiceCollection and Endpoint Configuration
The following example configures an NServiceBus endpoint within a ASP.Net Core MVC application to allow sending and receiving of messages.

Please note: Its required to populate the container with the currently registered services.

```c#
var container = new Container();

var endpointConfiguration = new EndpointConfiguration(Configuration.NServiceBus.Endpoint)
    .SetupAzureServiceBusTransport(true,() => Configuration.NServiceBus.ServiceBusConnectionString, r => { })
    .SetupLicense(Configuration.NServiceBus.LicenceText)
    .SetupInstallers()
    .SetupMsSqlServerPersistence(() => sp.GetService<DbConnection>())
    .SetupStructureMapBuilder(container)
    .SetupNewtonsoftSerializer()
    .SetupNLogFactory()
    .SetupOutbox()
    .SetupUnitOfWork();

services.AddNServiceBus(endpointConfiguration);

container.Populate(services);
```

#### Setup UnitOfWork Middleware
To enable the unit of work to begin/end the transaction, a middleware has been created to be used in the pipeline before the MVC middleware is registered, correctly configured it should look something like the following:

```c# 
app.UseStaticFiles()
    .UseErrorLoggingMiddleware()
    .UseSession()
    .UseAuthentication()
    .UseNserviceBusUnitOfWork()
    .UseMvc();
```