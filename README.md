# SFA Digital Apprenticeship Service Shared Packages

## Configuration

![badge](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/74/badge)

[![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.Configuration)](https://www.nuget.org/packages/SFA.DAS.Configuration/)

A shared library for storing configuration.

### Configuration - Azure table storage
Configuration is stored in a table called Configuration. The PartitionKey is the environment name, the RowKey is the service name and version number (Major.Minor) separated by an underscore. There is one extra column, Data, which is the Json representation of the configuration class for the service.

#### Debugging
To debug the package, you may have to add `https://symbols.nuget.org/download/symbols` to your symbol sources. More info [here](https://docs.microsoft.com/en-us/nuget/create-packages/symbol-packages-snupkg#nugetorg-symbol-server).

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

## SFA.DAS.Oidc.Middleware
Creates a NuGet package that is to be used for Code flow authentication. Once you have referenced the nuget package the following needs to configured in Startup


```csharp
app.UseCodeFlowAuthentication(new OidcMiddlewareOptions
{
        ClientId = "[CLIENT_ID]",
        ClientSecret = "[CLIENT_SECRET]",
        Scopes = "openid",
        BaseUrl = Constants.BaseAddress,
        TokenEndpoint = Constants.TokenEndpoint,
        UserInfoEndpoint = Constants.UserInfoEndpoint,
        AuthorizeEndpoint = Constants.AuthorizeEndpoint,
	AuthenticatedCallback = identity => { identity.AddClaim(new Claim("CustomClaim", "new claim added")); }
});

...

    public static class Constants
    {
        public const string BaseAddress = "[END_POINT_FOR_IDENTITY_URL]";

        public const string AuthorizeEndpoint = BaseAddress + "/Login/dialog/appl/oidctest/wflow/authorize";
        public const string LogoutEndpoint = BaseAddress + "/connect/endsession";
        public const string TokenEndpoint = BaseAddress + "/Login/rest/appl/oidctest/wflow/token";
        public const string UserInfoEndpoint = BaseAddress + "/Login/rest/appl/oidctest/wflow/userinfo";
        public const string IdentityTokenValidationEndpoint = BaseAddress + "/connect/identitytokenvalidation";
        public const string TokenRevocationEndpoint = BaseAddress + "/connect/revocation";
    }


```

And in Global.asax.cs

```csharp
AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
```

You are then able to use the [Authorize] attrbiute on any Controller Actions that you require authentication for

## Logging

Retrospective Note: DCM 148 Introduced breaking change: IRequestContext has been replaced with IWebLoggingContext

### NLog Target for Azure Event Hub - DAS.SFA.NLog.Targets.AzureEventHub
[link to README](Logging/SFA.DAS.NLog.Targets.AzureEventHub/README.MD)

### NLog Target for Azure Redis Hub - DAS.SFA.NLog.Targets.Redis
[link to README](Logging/SFA.DAS.NLog.Targets.Redis/README.MD)

##  Messaging
Provides a common interface to work with messaging sub-systems. Includes various implementations.

See [messaging readme](Messaging/README.md)!

## Http

[link to README](SFA.DAS.Http/README.md)
