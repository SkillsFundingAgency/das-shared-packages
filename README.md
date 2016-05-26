# SFA Digital Apprenticeship Service Shared Packages

## Configuration
A shared library for storing configuration.

### Configuration - Azure table storage
Configuration is stored in a table called Configuration. The PartitionKey is the environment name, the RowKey is the service name and version number (Major.Minor) separated by an underscore. There is one extra column, Data, which is the Json representation of the configuration class for the service.

## Time Provider
A shared library for making time-dependent functionality testable. You can download the NuGet package, SFA.DAS.TimeProvider, from nuget.org.

### Normal Usage
Replace calls in your codebase to **DateTime.UtcNow** with **DateTimeProvider.Current.UtcNow**

### Test Usage
Create a fake DateTimeProvider like the one below that can take a DateTime in it's constructor.
```csharp
public class FakeTimeProvider : DateTimeProvider
{
    public FakeTimeProvider(DateTime now)
    {
        UtcNow = now;
    }

    public override DateTime UtcNow { get; }
}
```

To set this fake provider as the current provider

```csharp
DateTimeProvider.Current = new FakeTimeProvider(DateTime.Now);
```

To return to the default provider,

```csharp
DateTimeProvider.ResetToDefault();
```

## CodeGenerator
Provides a way of generating a code that can be used for verification of a suer entering a website. The length of the desired code can be passed in. There is the option of making a numeric code or alphanumeric, the alphanumeric conforms to the DEC alphabet. It is also possible to pass your own implementation of the abstract class RandomNumberGenerator
