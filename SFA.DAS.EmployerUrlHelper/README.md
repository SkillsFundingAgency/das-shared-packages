# SFA.DAS.EmployerUrlHelper

For building links to employer applications. Alternatively see [SFA.DAS.ProviderUrlHelper] for building links to provider applications.

## Configuration

```c#
services.AddEmployerUrlHelper();
```

> Please note, a configuration section with key `SFA.DAS.EmployerUrlHelper` is required.

## Usage

The `ILinkGenerator` interface can be used for building links to employer applications:

```c#
var homepage = _linkGenerator.Homepage();
```

[SFA.DAS.ProviderUrlHelper]: https://github.com/SkillsFundingAgency/das-providerapprenticeshipsservice/tree/master/src/SFA.DAS.ProviderUrlHelper