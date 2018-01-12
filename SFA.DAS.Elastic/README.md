# SFA.DAS.Elastic

A library to help create a pre-configured [Elasticsearch .NET Client]. The first time a `Nest.IElasticClient` dependency is resolved the following will be taken care of for you:

* Connection pool setup.
* Debug logging.
* Exception throwing (by default [Elasticsearch .NET Client] will not throw exceptions for unsuccessful HTTP requests to the server).
* Automatic index creation.

## Requirements

1. Install [SFA.DAS.Elastic]:

```PowerShell
> Install-Package SFA.DAS.Elastic
```

2. Create an `SFA.DAS.Elastic.IElasticClientFactory` instance:

```C#
var clientFactory = new ElasticConfiguration()
    .UseSingleNodeConnectionPool("http://localhost:9200")
    .ScanForIndexMappers(typeof(Foo).Assembly)
    .OnRequestCompleted(r => Log.Debug(r.DebugInformation))
    .CreateClientFactory();
```

3. Register the `SFA.DAS.Elastic.IElasticClientFactory` & `Nest.IElasticClient` types as singletons with your container e.g. [StructureMap]:

```C#
var container = new Container(c =>
{
    c.For<IElasticClientFactory>().Use(clientFactory);
    c.For<IElasticClient>().Use(c => c.GetInstance<IElasticClientFactory>().CreateClient()).Singleton();
});
```

## Automatic index creation

Inheriting from `SFA.DAS.Elastic.IndexMapper<T>` allows you to define an individual index, specifically:

* A name for the index.
* A default POCO type for the index.
* Any mappings from the POCO type to the index.

e.g. For a POCO type of `Activity` auto mapped to an index named `activities`:

```
public class ActivitiesIndexMapper : IndexMapper<Activity>
{
    protected override string IndexName => "activities";

    protected override void Map(TypeMappingDescriptor<Activity> mapper)
    {
        mapper.AutoMap();
    }
}
```

Please note that the above will actually result in an index being created with a name prefixed with the environment name as per Ops' current requirements i.e. `local-activities`.

[Elasticsearch .NET Client]: https://www.nuget.org/packages/NEST
[SFA.DAS.Elastic]: https://www.nuget.org/packages/SFA.DAS.Elastic
[StructureMap]: https://www.nuget.org/packages/StructureMap