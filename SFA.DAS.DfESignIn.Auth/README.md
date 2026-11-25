# SFA.DAS.DfESignIn.Auth

[![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.DfESignIn.Auth)](https://www.nuget.org/packages/SFA.DAS.DfESignIn.Auth/)

Library for authenticating ASP.NET Core services with [DfE Sign-in](https://services.signin.education.gov.uk/) using OpenID Connect, creating an authentication cookie, and adding organisation and service-role claims.

The package can be used by a standalone service. It does not require the Provider shared UI.

## Pre-requisites

* .NET 10 and an ASP.NET Core web application.
* A DfE Sign-in client registration for each environment, including the permitted sign-in and sign-out callback URLs.
* The client ID and client secret for authentication.
* The DfE service ID and API secret when retrieving service roles.
* Azure Table Storage configuration, or another secure way of supplying the configuration values.
* A Redis connection for authentication tickets in a multi-instance environment. In-memory storage is available for a single instance, such as local development.

`Secret` and `APIServiceSecret` are separate credentials. `Secret` is the OpenID Connect client secret; `APIServiceSecret` signs requests to the DfE Sign-in public API.

## Configuration

Configuration is split between a shared section and a section for the selected client. The shared section is bound first, followed by the client section, producing one `DfEOidcConfiguration` object.

The following example shows the relevant sections for Assessor. Other client sections required by the existing shared configuration schema are omitted for brevity.

```json
{
  "DfEOidcConfiguration": {
    "BaseUrl": "https://test-oidc.signin.education.gov.uk",
    "ResponseType": "code",
    "KeyVaultIdentifier": "",
    "Scopes": "openid email profile organisation",
    "APIServiceUrl": "https://test-api.signin.education.gov.uk",
    "APIServiceAudience": "signin.education.gov.uk",
    "DfELoginSessionConnectionString": "",
    "LoginSlidingExpiryTimeOutInMinutes": 30
  },
  "DfEOidcConfiguration_Assessor": {
    "ClientId": "{CLIENT_ID}",
    "Secret": "{CLIENT_SECRET}",
    "APIServiceId": "{DFE_SERVICE_ID}",
    "APIServiceSecret": "{API_SECRET}",
    "CookieDomain": "",
    "SignedOutRedirectUrl": "/service/signed-out"
  }
}
```

The package reads the loaded `IConfiguration`; it does not load Azure Table Storage records itself. The consuming application must load the required configuration before registering authentication.

### Shared settings

| Setting | Purpose |
| --- | --- |
| `BaseUrl` | DfE Sign-in OIDC base URL for the environment. Supply it without a trailing slash. |
| `Scopes` | Space-separated scopes requested during authentication. |
| `APIServiceUrl` | DfE Sign-in public API base URL. |
| `DfELoginSessionConnectionString` | Redis connection string for authentication tickets. An empty or missing value selects distributed memory cache. |
| `LoginSlidingExpiryTimeOutInMinutes` | Sliding authentication expiry, in minutes. The configuration class defaults to `30`. |

The existing shared configuration schema also requires `ResponseType`, `KeyVaultIdentifier` and `APIServiceAudience`. The current implementation fixes the response type to `code`, uses `Secret` rather than `KeyVaultIdentifier` for client authentication, and uses the constant `signin.education.gov.uk` as the API token audience. Changing those three JSON values does not change that behaviour.

The cache implementation and its connection string are selected from the shared section during service registration.

### Client settings

| Setting | Purpose |
| --- | --- |
| `ClientId` | Client ID registered with DfE Sign-in. Also used as the issuer when signing public API requests. |
| `Secret` | OpenID Connect client secret. |
| `APIServiceId` | DfE service identifier used when retrieving a user's roles. |
| `APIServiceSecret` | Secret used to sign public API requests. |
| `CookieDomain` | Authentication cookie domain, without a scheme or path. An empty value produces a host-only cookie. |
| `SignedOutRedirectUrl` | Final destination after the DfE sign-out callback. This can be a local application path. |
| `Scopes` | Optional client-specific replacement for the shared scopes, where permitted by the configuration schema. |

For local development, use an empty `CookieDomain`. For a deployed service, set the required domain explicitly; the configuration-based overload does not calculate it from an environment name or client enum.

To override the scopes for Assessor, add the complete replacement value to its section:

```json
"Scopes": "openid email profile organisation"
```

Omit the property to inherit the shared scopes. A client value replaces the shared scope list; it is not appended to it.

The existing schema permits only the four credential properties in older client sections. Extend the relevant schema before adding cookie settings, sign-out settings or scope overrides to another client.

Registration throws an `ArgumentException` identifying the missing section if either the shared section or the selected client section is absent. Section existence checks do not validate every setting or run JSON Schema validation.

## Standard usage

After loading configuration, register authentication using the complete client section name:

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.AppStart;

namespace Example.Web.StartupExtensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddDfESignInAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAndConfigureDfESignInAuthentication(
                configuration,
                authenticationCookieName: "Example.Web.Auth",
                customServiceRole: typeof(CustomServiceRole),
                clientConfigurationSectionName: "DfEOidcConfiguration_Assessor",
                signedOutCallbackPath: "/account/sign-out");

            return services;
        }
    }
}
```

`CustomServiceRole` must implement `ICustomServiceRole`, as described below. Choose a cookie name appropriate to the application.

The consuming application must also configure its normal MVC services and endpoints, and include authentication and authorization middleware after routing and before endpoint execution:

```csharp
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
```

The package registers the following defaults:

| Operation | Scheme |
| --- | --- |
| Authenticate and sign in | `CookieAuthenticationDefaults.AuthenticationScheme` (`Cookies`) |
| Challenge and sign out | `OpenIdConnectDefaults.AuthenticationScheme` (`OpenIdConnect`) |

The authentication cookie is essential, secure, and uses sliding expiry. Authentication tickets are stored through `ITicketStore` using the configured distributed cache.

## ICustomServiceRole

Implement `ICustomServiceRole` to choose the claim type and whether an active DfE service role is represented by its name or code.

```csharp
using System.Security.Claims;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace Example.Web.StartupExtensions
{
    public class CustomServiceRole : ICustomServiceRole
    {
        public string RoleClaimType => ClaimTypes.Role;

        public CustomServiceRoleValueType RoleValueType =>
            CustomServiceRoleValueType.Code;
    }
}
```

This example adds each active role's code as a standard role claim. Applications can use their existing authorization policies to check those codes. The codes must match the roles defined for the service in DfE Sign-in.

`ICustomServiceRole` controls role mapping. It does not create an application user or synchronize an application database.

## User, organisation and role claims

DfE authentication uses the following claim names:

| Claim | Meaning |
| --- | --- |
| `sub` | DfE Sign-in user identifier. Use this to identify the person, independently of their selected organisation. |
| `email` | User's email address, requested through the `email` scope. |
| `given_name` and `family_name` | User profile names, requested through the `profile` scope. |
| `organisation` | JSON describing the selected organisation, requested through the `organisation` scope. |

Inbound claim mapping is disabled. Applications should use the DfE claim names rather than assume they have been converted to `ClaimTypes.NameIdentifier` or `ClaimTypes.Email`.

During token validation, `DfESignInService` reads the selected organisation and calls:

```text
GET {APIServiceUrl}/services/{APIServiceId}/organisations/{organisationId}/users/{userId}
```

It adds active service roles as `rolecode`, `roleId`, `roleName` and `rolenumericid`, as well as the claim selected by `ICustomServiceRole`.

It also adds display-name claims and `http://schemas.portal.com/ukprn`. UKPRN comes from the selected organisation; if the organisation has no UKPRN, the package uses `"0"`.

If no `organisation` claim is present, the enrichment method returns without adding organisation-derived or service-role claims. That method does not itself reject the login. The consuming application remains responsible for enforcing the required permissions.

A person can belong to multiple organisations and hold different service roles in each. With organisation selection enabled in DfE Sign-in, the package processes the single organisation returned for that authentication. It does not provide an application-level organisation switcher.

### Application-specific claims

The current `SFA.DAS.DfESignIn.Auth.Interfaces.ICustomClaims` interface is synchronous and is called by the legacy Provider stub with a null `TokenValidatedContext`. It is not automatically called by real DfE authentication.

It therefore does not currently provide the equivalent of the asynchronous custom-claims extension in `SFA.DAS.GovUK.Auth`. Application user creation and additional application claims require application integration.

## Signing in and signing out

Protect application endpoints using the normal ASP.NET Core authorization attributes or policies. An authentication challenge uses the registered OpenID Connect scheme.

An application can also expose explicit sign-in and sign-out actions:

```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Example.Web.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet("sign-in")]
        public IActionResult Login(string returnUrl = "/")
        {
            var redirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";

            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = redirectUri
                },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpPost("sign-out")]
        [ValidateAntiForgeryToken]
        public IActionResult SignOutUser()
        {
            return SignOut(
                new AuthenticationProperties(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
```

Submit sign-out using a form containing an anti-forgery token. Provide an anonymous application endpoint for the configured `SignedOutRedirectUrl` so the signed-out landing page does not immediately challenge the user again.

The paths serve different purposes:

| Setting or path | Purpose |
| --- | --- |
| `/sign-in` | OIDC sign-in callback handled by authentication middleware. Register the full URL with DfE Sign-in. |
| `/signout-callback-oidc` | OIDC sign-out callback handled by authentication middleware. Register the full URL with DfE Sign-in. |
| `signedOutCallbackPath` registration argument | Despite its existing name, this sets the cookie scheme's `LogoutPath`. It does not change the OIDC sign-out callback or create a controller action. |
| `SignedOutRedirectUrl` | Final destination after the OIDC sign-out callback, such as `/service/signed-out`. |

The example uses `/account/sign-in` for the application's sign-in action so it does not conflict with the package's `/sign-in` OIDC callback.

## Legacy ClientName registration

The overload accepting `ClientName` remains available for existing consumers but is deprecated:

```csharp
services.AddAndConfigureDfESignInAuthentication(
    configuration,
    "Example.Provider.Auth",
    typeof(CustomServiceRole),
    ClientName.ProviderRoatp,
    "/sign-out",
    "");
```

The legacy overload uses the package-owned enum to select `DfEOidcConfiguration_{ClientName}` and calculate cookie domains and default sign-out destinations from environment-specific mappings. `RoatpServiceAdmin` uses the `ServiceAdmin` credentials section.

This couples the shared package to individual clients: adding a client requires an enum or domain-mapping change and a package release. New consumers should supply the client configuration section name and configure their domain and sign-out destination explicitly.

To migrate an existing client:

1. Extend its configuration schema to permit `CookieDomain` and `SignedOutRedirectUrl`, and `Scopes` if a client override is required.
2. Populate the domain and sign-out destination with the values required by that service in each environment.
3. Replace the enum argument with the complete client section name.
4. Supply the cookie logout path through `signedOutCallbackPath`; move any explicit legacy `redirectUrl` value into `SignedOutRedirectUrl`.
5. Verify sign-in, service-role authorization, cookie scope and sign-out in the consuming application.

The enum-based overloads of `AddServiceRegistration`, `ConfigureDfESignInAuthentication` and `AddAuthenticationCookie` are also deprecated in favour of explicit configuration.

## Stub authentication

### Existing Provider stub

The legacy enum-based registration selects the existing Provider stub when the root `NoAuthEmail` setting is non-empty. The setting acts as a switch; its value is not used as the user's email by the stub handler.

This stub automatically authenticates requests with fixed claims:

| Claim | Value |
| --- | --- |
| Name | `10000001` |
| `http://schemas.portal.com/displayname` | `APIM Provider User` |
| `http://schemas.portal.com/service` | `DAA` |
| `http://schemas.portal.com/ukprn` | `10000001` |

It can append claims through the synchronous `ICustomClaims.GetClaims(null)` method. The consuming application must register that implementation and `IHttpContextAccessor` for the handler's dependencies.

There is no login form or organisation chooser. The handler supplies no `sub` or `email` claim by default. Sign-out redirects to `/`, but subsequent requests are automatically authenticated again. This legacy switch has no production-environment restriction in the current implementation.

### Configuration-based registration

The new configuration-based registration currently enables real DfE authentication. It does not select the legacy Provider stub through `NoAuthEmail`.

The interactive stub that accepts a user ID and email and then offers configured organisations has not yet been implemented. Razor view compilation is enabled to support adding those screens to this package; it does not itself provide or enable stub endpoints. No interactive `StubAuth` configuration is available yet.

## Building and testing

From the `das-shared-packages` repository root:

```shell
dotnet build SFA.DAS.DfESignIn.Auth/SFA.DAS.DfESignIn.Auth/SFA.DAS.DfESignIn.Auth.csproj

dotnet test SFA.DAS.DfESignIn.Auth/SFA.DAS.DfESignIn.Auth.UnitTests/SFA.DAS.DfESignIn.Auth.UnitTests.csproj

dotnet pack SFA.DAS.DfESignIn.Auth/SFA.DAS.DfESignIn.Auth/SFA.DAS.DfESignIn.Auth.csproj --configuration Release --output ./artifacts
```

## Further information

* [DfE Sign-in help](https://help.signin.education.gov.uk/)
* [DfE Sign-in public API documentation](https://github.com/DFE-Digital/login.dfe.public-api)
* [Test OpenID Connect discovery metadata](https://test-oidc.signin.education.gov.uk/.well-known/openid-configuration)
* [Production OpenID Connect discovery metadata](https://oidc.signin.education.gov.uk/.well-known/openid-configuration)
