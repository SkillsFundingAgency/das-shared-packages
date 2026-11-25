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

The configuration-based registration requires both the shared section and the selected client section. Legacy real authentication also requires its selected client section, while the legacy automatic Provider stub does not. Registration throws an `ArgumentException` identifying a required section that is missing. Section existence checks do not validate every setting or run JSON Schema validation.

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
                signOutPath: "/account/sign-out");

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

For real authentication, the package registers the following defaults:

| Operation | Scheme |
| --- | --- |
| Authenticate and sign in | `CookieAuthenticationDefaults.AuthenticationScheme` (`Cookies`) |
| Challenge and sign out | `OpenIdConnectDefaults.AuthenticationScheme` (`OpenIdConnect`) |

The authentication cookie is essential, secure, and uses sliding expiry. Authentication tickets are stored through `ITicketStore` using the configured distributed cache. The new interactive stub uses `Cookies` for authenticate, sign-in, challenge and sign-out; it uses the same ticket-store configuration and appends `.stub` to the supplied cookie name.

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

Applications can register an implementation of `SFA.DAS.DfESignIn.Auth.Interfaces.ICustomClaims`:

```csharp
services.AddScoped<ICustomClaims, CustomClaims>();
```

| Registration path | Custom claims behaviour |
| --- | --- |
| Legacy enum registration, real authentication | Does not invoke `ICustomClaims`. |
| Legacy automatic Provider stub | Calls `GetClaims(null)` synchronously. |
| Configuration-based registration, real authentication | Awaits `GetClaimsAsync()` after user-info processing and before issuing the authentication cookie. |
| Configuration-based registration, interactive stub | Awaits `GetClaimsAsync()` after constructing the selected user's claims and before issuing the authentication cookie. |

The async handler is resolved from the current request's service provider. It is optional: authentication continues without additional application claims when no implementation is registered.

Use `context.Principal` to read `sub`, `email` and the selected organisation. The async hook supplies a constructed `TokenValidatedContext`; its security-token and token-endpoint-response properties are not populated.

Both interface methods must be implemented. The following minimal example supports the legacy stub and both new authentication paths without duplicating the claims:

```csharp
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace Example.Web.StartupExtensions
{
    public class CustomClaims : ICustomClaims
    {
        public IEnumerable<Claim> GetClaims(
            TokenValidatedContext tokenValidatedContext)
        {
            return new List<Claim>
            {
                new Claim("example:custom-claims", "added")
            };
        }

        public Task<IEnumerable<Claim>> GetClaimsAsync(
            TokenValidatedContext tokenValidatedContext)
        {
            return Task.FromResult(GetClaims(tokenValidatedContext));
        }
    }
}
```

Applications that create or update a user through an API should await that work inside `GetClaimsAsync()` and return the resulting application claims. The synchronous wrapper shown above does not make synchronous I/O asynchronous.

`GetClaimsAsync()` must return a non-null collection, which may be empty. Returned claims are added to the resulting principal. A replacement principal must contain a `ClaimsIdentity`.

A handler can redirect the response and call `context.HandleResponse()` to stop normal sign-in. Calling `Fail()`, calling `SkipHandler()`, or throwing an exception also prevents the normal authentication cookie from being issued.

## Signing in and signing out

Protect application endpoints using the normal ASP.NET Core authorization attributes or policies. Let challenges use the registered default scheme so the same application can use either real authentication or the new interactive stub.

The following example also reads the effective default sign-out scheme. This follows the authentication mode actually registered by the package, including when production configuration prevents stub mode.

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Example.Web.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

        public AccountController(
            IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        [AllowAnonymous]
        [HttpGet("sign-in")]
        public IActionResult Login(string returnUrl = "/")
        {
            var redirectUri = Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : Url.Content("~/");

            return Challenge(new AuthenticationProperties
            {
                RedirectUri = redirectUri
            });
        }

        [HttpPost("sign-out")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOutUser()
        {
            var signOutScheme = await _authenticationSchemeProvider
                .GetDefaultSignOutSchemeAsync();

            if (signOutScheme == null)
            {
                throw new InvalidOperationException(
                    "No default sign-out scheme has been configured.");
            }

            if (signOutScheme.Name ==
                CookieAuthenticationDefaults.AuthenticationScheme)
            {
                return SignOut(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return SignOut(
                new AuthenticationProperties(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                signOutScheme.Name);
        }
    }
}
```

For this example, submit sign-out using a form containing an anti-forgery token:

```cshtml
<form asp-controller="Account"
      asp-action="SignOutUser"
      method="post"
      asp-antiforgery="true">
    <button type="submit" class="govuk-button">Sign out</button>
</form>
```

Provide an anonymous application endpoint for the configured `SignedOutRedirectUrl` so the signed-out landing page does not immediately challenge the user again.

The paths serve different purposes:

| Setting or path | Purpose |
| --- | --- |
| `/sign-in` | OIDC sign-in callback handled by authentication middleware. Register the full URL with DfE Sign-in. |
| `/signout-callback-oidc` | OIDC sign-out callback handled by authentication middleware. Register the full URL with DfE Sign-in. |
| `signOutPath` registration argument | Sets the cookie scheme's `LogoutPath`, normally matching the application's action that initiates sign-out. It does not change the OIDC callback, create a controller action or automatically perform sign-out. |
| `SignedOutRedirectUrl` | Final destination after the OIDC sign-out callback, such as `/service/signed-out`. The new stub redirects here directly when signing out. |

The example uses `/account/sign-in` for the application's sign-in action so it does not conflict with the package's `/sign-in` OIDC callback.

Do not decide which schemes to sign out solely by rereading `StubAuth`: it can be true while the package has selected real authentication because the environment is production.

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
4. Supply the cookie logout path through `signOutPath`; move any explicit legacy `redirectUrl` value into `SignedOutRedirectUrl`.
5. Verify sign-in, service-role authorization, cookie scope and sign-out in the consuming application.

The legacy public `AddAuthenticationCookie` overload accepting an environment and `ClientName` is also deprecated in favour of the overload accepting an explicit cookie domain.

## Upgrading existing consumers

This release includes source and interface changes; it is not a drop-in compatible upgrade for every consumer.

1. Implement the additional `ICustomClaims.GetClaimsAsync()` method wherever the DfE interface is implemented.
2. Rename named arguments from `signedOutCallbackPath:` to `signOutPath:`. Positional calls are unaffected by that rename.
3. Ensure legacy real authentication has its selected client configuration section. A missing section now fails during registration; the legacy automatic Provider stub does not require that section.

An existing synchronous claims implementation can add:

```csharp
public Task<IEnumerable<Claim>> GetClaimsAsync(
    TokenValidatedContext tokenValidatedContext)
{
    return Task.FromResult(GetClaims(tokenValidatedContext));
}
```

Implementations performing asynchronous I/O should await that work inside `GetClaimsAsync()`. Rebuild assemblies implementing the interface against the updated package.

The legacy enum-based real authentication path retains its previous custom-claims behaviour: it does not invoke the application's handler. Switching to configuration-based registration enables the async hook for real authentication, so review any existing stub-only claims before migrating.

Moving from the legacy automatic stub to the interactive stub also requires the new client-section registration, `StubAuth`, configured stub users and MVC controller discovery. The two stub mechanisms use different switches and authentication behaviour.

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

The configuration-based overload uses the interactive stub when the consuming application sets `StubAuth` to `true`, unless `ResourceEnvironmentName` is `PRD` (case-insensitive). In that production case, real DfE authentication is registered.

`StubAuth` and `DfESignInStubConfiguration` belong to the consuming application's configuration, separately from the shared DfE credentials configuration. This overload does not use `NoAuthEmail`.

The shared `DfEOidcConfiguration` and selected client section are still required in stub mode. The stub uses the configured client ID for role claim issuers and the configured cookie and sign-out settings, but does not contact the live DfE service.

The following example contains fictitious users and organisations:

```json
{
  "StubAuth": true,
  "DfESignInStubConfiguration": {
    "DefaultUserId": "11111111-1111-1111-1111-111111111111",
    "Users": [
      {
        "Id": "11111111-1111-1111-1111-111111111111",
        "Email": "user.one@example.com",
        "GivenName": "User",
        "FamilyName": "One",
        "Organisations": [
          {
            "Id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
            "Name": "Organisation A",
            "UkPrn": 10000001,
            "Roles": [
              {
                "Id": "33333333-3333-3333-3333-333333333333",
                "Name": "Administrator",
                "Code": "ADMIN",
                "NumericId": 1,
                "Status": {
                  "Id": 1
                }
              }
            ]
          },
          {
            "Id": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
            "Name": "Organisation B",
            "UkPrn": 10000002,
            "Roles": []
          }
        ]
      },
      {
        "Id": "22222222-2222-2222-2222-222222222222",
        "Email": "user.two@example.com",
        "GivenName": "User",
        "FamilyName": "Two",
        "Organisations": [
          {
            "Id": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
            "Name": "Organisation B",
            "UkPrn": 10000002,
            "Roles": [
              {
                "Id": "44444444-4444-4444-4444-444444444444",
                "Name": "Read only",
                "Code": "READ",
                "NumericId": 2,
                "Status": {
                  "Id": 1
                }
              }
            ]
          }
        ]
      }
    ]
  }
}
```

`DefaultUserId` selects the initial user on the chooser. Each user's `Organisations` list defines their memberships. Roles belong to that user's membership, allowing different users to hold different roles in the same organisation. Include each role's ID, name, code, numeric ID and status; only roles with `Status.Id = 1` are added to the principal.

Include the package's controller in MVC registration:

```csharp
using SFA.DAS.DfESignIn.Auth.Controllers;

builder.Services
    .AddControllersWithViews()
    .AddApplicationPart(typeof(DfESignInStubController).Assembly);
```

If MVC is already registered, add `AddApplicationPart` to that existing registration. The default views use the consuming application's `_Layout` and GOV.UK CSS classes.

The sign-in journey is:

1. An authentication challenge redirects to `/stub/sign-in-stub`.
2. The user chooses a configured identity.
3. `/stub/select-organisation` offers that user's configured organisations.
4. The POST to `/stub/sign-in` validates the selected membership and builds the principal.
5. The package invokes the application's async custom-claims handler, if registered, and issues the authentication cookie.

The POST requires an anti-forgery token. Identity, email, organisation and roles are read from server-side configuration, and the final return URL must be local.

Generated claims include `sub`, `email`, `given_name`, `family_name`, organisation JSON, name, display name, UKPRN and active service roles. Display name is constructed from the configured given and family names. Organisation JSON contains `id`, `name` and `ukprn`; UKPRN is serialised as a string, or null when absent. The separate UKPRN claim uses `"0"` when absent.

The stub does not manufacture the complete organisation object returned by real DfE Sign-in, or a DfE session identifier. Extend the stub data when a consuming application needs additional organisation fields.

Sign-out clears the local authentication session and redirects to `SignedOutRedirectUrl`. A subsequent authentication challenge starts the chooser journey again, allowing a different user or organisation to be selected.

## SampleSite

The sample is located at:

```text
SFA.DAS.DfESignIn.Auth.Samples/SFA.DAS.DfESignIn.SampleSite
```

It references the authentication project directly. No local NuGet publishing is needed to exercise changes to the package.

The sample loads Azure Table Storage configuration using `ConfigNames`, `ConfigurationStorageConnectionString` and `Environment` before registering authentication. Environment variables are applied afterwards so launch-profile settings can override the loaded configuration.

The checked-in storage connection uses Azurite. Populate the required local configuration records from the private configuration repository, including the client section selected in `AddServiceRegistrationExtension`. Keep actual DfE credentials and real storage-account credentials out of the sample's checked-in settings.

Launch profiles select:

| Profile | Setting |
| --- | --- |
| `DfE Sign-in - Stub` | `StubAuth=true` |
| `DfE Sign-in - Test server` | `StubAuth=false` |

The configured DfE endpoint and client registration determine the real authentication environment; the launch profile's name does not select a DfE server.

Both profiles use `https://localhost:5021`. The Provider shared UI dashboard URL must match the local address. The sample supplies the Provider UI's required named sign-out route, `provider-signout`.

The authenticated page lists the principal's claims. The sample custom-claims handler adds `sample:custom-claims = added`, demonstrating that the async hook runs for both real and interactive stub sign-in.

Verify the sample manually:

1. Start the stub profile and select a configured user and organisation.
2. Check the identity, organisation, UKPRN, role and custom claims.
3. Sign out using the header link and confirm the signed-out destination.
4. Sign in again and select another user or organisation.
5. Start the real test-server profile and verify real sign-in, claims and sign-out using the configured DfE test client.

No SampleSite unit tests are required; package behaviour is covered by the authentication unit tests.

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

