# SFA.DAS.UnitOfWork

This package includes:

* Infrastructure to support transactional units of work.
* Extension methods for NServiceBus, MVC, WebApi to wire up transactional behaviour e.g. Creating a DB transaction at the beginning of an HTTP request and committing it at the end of an HTTP request.

## Configuration

### NServiceBus

For examples of how to configure NServiceBus client and server endpoints to support transactional units of work please see the samples:

* [SFA.DAS.UnitOfWork.Sample.MessageHandlers]
* [SFA.DAS.UnitOfWork.Sample.Web]

### MVC Core

The following example configures an application to:

1. Create a SQL Server transaction at the beginning of an HTTP request.
2. Commit the SQL Server transaction at the end of the HTTP request.

```c#
services.AddSqlServerUnitOfWork();
app.UseUnitOfWork();
```

A reference to the SQL Server connection and/or transaction can be retrieved from the `IUnitOfWorkContext`:

```c#
var connection = unitOfWorkContext.Get<DbConnection>();
var transaction = unitOfWorkContext.Get<DbTransaction>();
```

### MVC

The following example configures an application to:

1. Create a SQL Server transaction at the beginning of an HTTP request.
2. Commit the SQL Server transaction at the end of the HTTP request.

```c#
var container = new Container(c => c.AddRegistry<SqlServerUnitOfWorkRegistry>());

filters.AddUnitOfWorkFilter();
```

A reference to the SQL Server connection and/or transaction can be retrieved from the `IUnitOfWorkContext`:

```c#
var connection = unitOfWorkContext.Get<DbConnection>();
var transaction = unitOfWorkContext.Get<DbTransaction>();
```

### WebApi

The following example configures an application to:

1. Create a SQL Server transaction at the beginning of an HTTP request.
2. Commit the SQL Server transaction at the end of the HTTP request.

```c#
var container = new Container(c => c.AddRegistry<SqlServerUnitOfWorkRegistry>());

filters.AddUnitOfWorkFilter();
```

A reference to the SQL Server connection and/or transaction can be retrieved from the `IUnitOfWorkContext`:

```c#
var connection = unitOfWorkContext.Get<DbConnection>();
var transaction = unitOfWorkContext.Get<DbTransaction>();
```

[SFA.DAS.UnitOfWork.Sample.MessageHandlers]: https://github.com/SkillsFundingAgency/das-shared-packages/tree/master/SFA.DAS.UnitOfWork/SFA.DAS.UnitOfWork.Sample.MessageHandlers
[SFA.DAS.UnitOfWork.Sample.Web]: https://github.com/SkillsFundingAgency/das-shared-packages/tree/master/SFA.DAS.UnitOfWork/SFA.DAS.UnitOfWork.Sample.Web