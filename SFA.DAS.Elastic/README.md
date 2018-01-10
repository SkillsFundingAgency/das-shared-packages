# SFA.DAS.Elastic

A library to register a pre-configured [Elasticsearch .NET Client] with a [StructureMap] container. The first time a `Nest.IElasticClient` dependency is resolved the following will be taken care of for you:

* Singleton lifetime.
* Connection pool setup (using `SingleNodeConnectionPool` as per Ops' current requirements).
* Automatic debug logging (using `SFA.DAS.NLog.Logger.ILog`).
* Automatic exception throwing (by default [Elasticsearch .NET Client] will not throw exceptions for unsuccessful HTTP requests to the server).
* Automatic index creation.

## Requirements

1. Install [SFA.DAS.Elastic]:

```PowerShell
> Install-Package SFA.DAS.Elastic
```

2. Update your JSON config to include the following properties (e.g. [SFA.DAS.Activities.json](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-activities/SFA.DAS.Activities.json)):

   * `ElasticUrl`
   * `ElasticUsername`
   * `ElasticPassword`

3. Update your C# config to inherit from `IElasticConfiguration` to include the above properties.
4. Add the registry to your container e.g.:

```C#
_container = new Container(c =>
{
    c.AddRegistry<ElasticRegistry>();
});
```

5. Add the config to your container e.g.:
```C#
_container = new Container(c =>
{
    c.For<IElasticConfiguration>().Use(config);
});
```

## Automatic index creation

Inheriting from `SFA.DAS.Elastic.IndexMapper<T>` allows you to define an individual index, specifically:

* A name for the index.
* A default POCO type for the index.
* Any mappings from the POCO type to the index.

e.g. For an index named `activities` with default mapping:

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

Please note that the above will actually result in an index being created with a name prefixed with the environment name as per Ops' current requirements i.e. `at-activities`.

Any classes in your assembly that inherit from `SFA.DAS.Elastic.IndexMapper<T>` will be registered automatically with StructureMap and will be used to create your indexes, if they don't already exist, the first time a `Nest.IElasticClient` dependency is resolved.

[Elasticsearch .NET Client]: https://www.nuget.org/packages/NEST
[SFA.DAS.Elastic]: https://www.nuget.org/packages/SFA.DAS.Elastic
[StructureMap]: https://www.nuget.org/packages/StructureMap