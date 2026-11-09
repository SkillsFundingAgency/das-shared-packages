## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# SFA.DAS.Testing

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status%2FShared%20Packages%2Fdas-shared-SFA.DAS.Testing?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=3692&branchName=master)


[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

## About

Provides a single location for common types across multiple SFA DAS solutions.

Code placed in here should represent truly global facts or data structures that can be used everywhere and that should be managed in one place.

-------

## 🚀 Installation

### Pre-Requisites
* A clone of this repository
* An appropriate IDE


## Technologies
* .NetCore 10.0
* NUnit
* Moq
* FluentAssertions

### Build

Open the solution in your C# IDE of choice and build the project.

---

## Packages

This folder publishes two NuGet packages.

### SFA.DAS.Testing

Core testing infrastructure and helpers shared across DAS services.

| | |
|---|---|
| **NuGet** | [![NuGet](https://img.shields.io/nuget/v/SFA.DAS.Testing.svg)](https://www.nuget.org/packages/SFA.DAS.Testing/) |
| **Target framework** | .NET Standard 2.0 |
| **Dependency** | `Microsoft.AspNetCore.Mvc >= 2.2.0` |

### SFA.DAS.Testing.AutoFixture

AutoFixture extensions and custom attributes for use with NUnit3 and Moq in DAS test projects.

| | |
|---|---|
| **NuGet** | [![NuGet](https://img.shields.io/nuget/v/SFA.DAS.Testing.AutoFixture.svg)](https://www.nuget.org/packages/SFA.DAS.Testing.AutoFixture/) |
| **Target framework** | .NET 8.0+ |
| **Dependencies** | `AutoFixture.AutoMoq >= 4.18.1`, `AutoFixture.NUnit3 >= 4.18.1`, `System.Net.Http >= 4.3.4`, `System.Text.RegularExpressions >= 4.3.1` |

---

## Installation

### .NET CLI

```bash
# Core testing infrastructure
dotnet add package SFA.DAS.Testing

# AutoFixture extensions
dotnet add package SFA.DAS.Testing.AutoFixture
```

### PackageReference

```xml
<PackageReference Include="SFA.DAS.Testing" Version="3.0.169" />
<PackageReference Include="SFA.DAS.Testing.AutoFixture" Version="17.1.165" />
```

---

## Usage

### SFA.DAS.Testing

Provides base classes, MVC test helpers and shared test infrastructure for DAS services. Typical usage involves inheriting from base test classes or using the included assertion/mock helpers in NUnit test projects alongside Moq and FluentAssertions.

### SFA.DAS.Testing.AutoFixture

Provides custom AutoFixture attributes and extensions designed for the DAS test stack (NUnit3 + Moq). These make it straightforward to inject auto-generated and auto-mocked dependencies directly into test methods.

**Example – using the `[MoqAutoData]` attribute:**

```csharp
using AutoFixture.NUnit3;
using SFA.DAS.Testing.AutoFixture;
using NUnit.Framework;

[TestFixture]
public class MyServiceTests
{
    [Test, MoqAutoData]
    public void DoSomething_ShouldReturnExpectedResult(
        [Frozen] Mock<IMyRepository> repository,
        MyService sut,
        MyModel expected)
    {
        repository.Setup(r => r.GetAsync(expected.Id))
                  .ReturnsAsync(expected);

        var result = sut.DoSomething(expected.Id);

        result.Should().BeEquivalentTo(expected);
    }
}
```

---

## Related Packages

Other shared packages in `das-shared-packages` you may need alongside this one:

| Package | Purpose |
|---|---|
| `SFA.DAS.NServiceBus.Testing` | Test stubs for NServiceBus |
| `SFA.DAS.CosmosDb.Testing` | Test stubs for CosmosDb |
| `SFA.DAS.Configuration` | Azure Table Storage configuration |
| `SFA.DAS.Http` | Shared HTTP client infrastructure |

---

## License

Licensed under the [MIT license](LICENSE)

[MIT](https://github.com/SkillsFundingAgency/das-shared-packages/blob/master/LICENSE)

# SFA.DAS.Testing

> Testing infrastructure for the [Skills Funding Agency](https://github.com/SkillsFundingAgency) Digital Apprenticeship Service (DAS) platform.

Part of the [das-shared-packages](https://github.com/SkillsFundingAgency/das-shared-packages) monorepo.