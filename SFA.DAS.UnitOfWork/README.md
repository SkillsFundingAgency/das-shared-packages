# SFA.DAS.UnitOfWork

This package includes:

* Infrastructure to support transactional units of work.
* Extension methods for `Mvc`, `WebApi` and `NServiceBus` to wire up transactional behaviour i.e. Creating a DB transaction at the beginning of an HTTP request and committing it at the end of an HTTP request.
* Registries for `StructureMap` to wire up unit of work behaviour i.e. Publishing events at the end of a HTTP request.

## Configuration

The following example configures an MVC application to:

1. Create a SQL Server transaction at the beginning of an HTTP request.
2. Save changes using an Entity Framework `DbContext` at the end of an HTTP request.
3. Commit the SQL Server transaction at the end of the HTTP request.

### MVC

```c#
var container = new Container(c =>
{
    c.AddRegistry<EntityFrameworkUnitOfWorkRegistry<FooDbContext>>();
    c.AddRegistry<SqlServerUnitOfWorkRegistry>();
});

filters.AddUnitOfWorkFilter();
```

### MVC Core

```c#
var container = new Container(c =>
{
    c.AddRegistry<EntityFrameworkUnitOfWorkRegistry<FooDbContext>>();
    c.AddRegistry<SqlServerUnitOfWorkRegistry>();
});

app.UseUnitOfWork();
```

## Extension

If you're not using SQL Server then the infrastructure can be extended easily to accomodate your stack. For example, if you're using RavenDB then you'll just need to add a custom implementation of `IUnitOfWorkManager`. `IUnitOfWorkManagers`'s signature currently looks like this:

```c#
public interface IUnitOfWorkManager
{
    Task BeginAsync();
    Task EndAsync(Exception ex = null);
}
```
