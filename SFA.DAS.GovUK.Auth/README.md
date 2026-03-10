# SFA.DAS.GovUK.Auth

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status%2FShared%20Packages%2Fdas-shared-packages-SFA.DAS.GovUK.Auth?repoName=SkillsFundingAgency%2Fdas-shared-packages&branchName=refs%2Fpull%2F703%2Fmerge)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2923&repoName=SkillsFundingAgency%2Fdas-shared-packages&branchName=refs%2Fpull%2F703%2Fmerge)

[![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.GovUK.Auth)](https://www.nuget.org/packages/SFA.DAS.GovUK.Auth/)

Library to enable employer/citizen facing service to use [Gov One Login](https://www.sign-in.service.gov.uk/) for authentication.


## GovUK Auth

### Pre-requisites 

* Gov One Login client registration
* Key Vault storage - to store the private key with managed identity authentication
* Azure Table Storage, or a secure way of storing config values

## Configuration

```json
{
  "GovUkOidcConfiguration": {
    "BaseUrl": "https://{INTEGRATION_ENVIRONMENT_URL}.gov.uk",    // From Gov One Login 
    "ClientId": "{CLIENT_ID}",    // From Gov One Login
    "KeyVaultIdentifier": "https://{YOUR_KEYVAULT}.vault.azure.net/keys/{KEY_NAME}",
    "LoginSlidingExpiryTimeOutInMinutes" : 30,
    "GovLoginSessionConnectionString" : "RedisConnectionString",
    "Disable2Fa" : "false" // Defaults to false if not set - true disables 2fa on sign-in.
  }
}
```

The above is the minimum required configuration; for the employer apprenticeship service it is stored in a shared configuration key `SFA.DAS.Employer.GovSignIn_1.0` and using 
the Azure Table storage package, can be included in the config names required for the application. If `GovLoginSessionConnectionString` is not set, the library will fall 
back to DistributedMemoryCache. This is suitable only for single-instance environments (e.g. local development).

The above configuration can be used for any service which does not require a Verify level of confidence, i.e. access to the verified name and date of birth of the user.

```json
{
  "GovUkOidcConfiguration": {
    "BaseUrl": "https://{INTEGRATION_ENVIRONMENT_URL}.gov.uk",    // From Gov One Login 
    "ClientId": "{CLIENT_ID}",                                    // From Gov One Login
    "KeyVaultIdentifier": "https://{YOUR_KEYVAULT}.vault.azure.net/keys/{KEY_NAME}",
    "LoginSlidingExpiryTimeOutInMinutes" : 30,
    "GovLoginSessionConnectionString" : "RedisConnectionString",
    "UseStubRedisTicketStore" : "true", // Defaults to false if not set - true uses Redis as authentication ticket store for stub sign in
    "Disable2Fa" : "false", // Defaults to false if not set - true disables 2fa on sign-in.
    "EnableVerify": "false", // Defaults to false if not set - true to verify on first sign-in, if true Disable2Fa must be set to false
    "RequestedUserInfoClaims": "CoreIdentityJWT" // Must be present for verify to succeed
  }
}
```

The above is a minimum extended configuration which is used for a service which requires the verify level of confidence. The `EnableVerify` when set to true indicates that verify is required on first sign-in 
to the service, when `EnableVerify` is false then verify can be requested later by marking an endpoint with the `IsVerified` policy. The `RequestedUserInfoClaims` are the verify information which
is set in the claims via the UserInfo endpoint; valid values are CoreIdentityJWT, Address, Passport and DrivingPermit. If `EnableVerify` is set to true or the `IsVerified` policy is checked then
at least CoreIdentityJWT must be specified in `RequestedUserInfoClaims` or the `IsVerified` policy will fail.

Notes:

When Verify is requested after the first sign-in, additional claims are retrieved from the UserInfo endpoint and added to the authentication ticket.

Because claims must be updated after the initial authentication, the ticket must be stored in a mutable store (Redis); Gov One Login has always used Redis as the authentication ticket store,
however Stub authentication can now optionally use Redis to support verify functionality but this must be enabled using `UseStubRedisTicketStore`.

The `UseStubRedisTicketStore` setting enables Redis-backed authentication tickets for Stub authentication, this is required when claims must be updated after login (for example when Verify is requested). 

**If multiple services share the same authentication cookie domain, they must all use the same ticket storage mechanism (cookie or Redis).** Otherwise users may experience authentication redirect loops between services. In practice this 
means that if Verify is required in one service all services sharing the same authentication cookie domain must be using Redis as the ticket store, this is always the case for Gov One Login, but for the 
Stub all services would need updating to a package version with Verify included and each one would need the `UseStubRedisTicketStore` set to true.

The claims which are specified during the Gov One Login client registration must include at least those in `RequestedUserInfoClaims` or these claims will not be present after a 
verify level of confidence is requested and granted.

## ICustomClaims

At the end of authentication process after the security token has been validated, there is the ability to add custom claims for use within your application. This relies on you
implementing the `ICustomClaims` interface, and adding any required claims to the identity, an example is shown below:

```csharp
public class CustomClaims : ICustomClaims
{
    public async Task<IEnumerable<Claim?>> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        var value = tokenValidatedContext?.Principal?.Identities.First().Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))
            ?.Value;
        return new List<Claim>
        {
            new Claim("MySpecialId",$"ABC123"),
            new Claim(ClaimTypes.Name, $"Mr Test Tester")
        };
    }
}
```
At this point you have the option to call any services in your code to add extra claim information that is required. The **NameIdentifier** and **Email** claims are always populated. Please note that
this is executed once during the authentication process when the token is validated, if you need to update claims you'll need to log out then log in again.

## IGovAuthEmployerAccountService

To standardise the claims that come back for use with employer accounts, the `ICustomClaims` option should be set to null, and the 
`IGovAuthEmployerAccountService` should be implemented returning the EmployerUserAccounts.

## Standard Usage
The package is designed to be used with the `SFA.DAS.Employer.Shared.UI`, and also `SFA.DAS.EmployerProfiles.Web`. Many of the default redirect URLs will go to the employer profiles site. 

After the configuration has been loaded, the following is required to configure Gov One Login. It is assumed that the `GovUKOidcConfiguration` object has been loaded and is available in `IConfiguration`. You are 
then able to specify the implementation of `ICustomClaims`, an override to the signout redirect URL, and also where the Stub Authentication login action is.


```csharp
public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAndConfigureGovUkAuthentication(
            configuration, 
            new AuthRedirects 
            {
                LoginRedirect = "/home/AccountDetails", //Where to redirect after login
                CookieDomain = "employer.gov.uk", //If using subsites for a client this should be the same across all
                LocalStubLoginPath = "/home/stub-account-details", // detailed below - stub auth action
                SignedOutRedirectUrl = "/home/signout" //where user is redirected to after signout, this shouldnt be the same as configured in gov login
            }, 
            typeof(CustomClaims), // Must implement ICustomClaims
            typeof(UserAccountService) ); // Must implement IGovAuthEmployerAccountService
             
    }
}    
```
After successful authentication and the access code being exchanged for a token, the **Email** and **NameIdentifier** claims are populated. An authentication cookie is created which has a sliding expiry set to `LoginSlidingExpiryTimeOutInMinutes`.

To sign out the following should be called:

```csharp
    public async Task<IActionResult> SigningOut()
    {
        var idToken = await HttpContext.GetTokenAsync("id_token");

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.Parameters.Clear();
        authenticationProperties.Parameters.Add("id_token",idToken);
        
        return SignOut(
            authenticationProperties, 
            new[] {
                CookieAuthenticationDefaults.AuthenticationScheme, 
                OpenIdConnectDefaults.AuthenticationScheme});
    }
```
It is necessary to pass the id_token which will exist as part of the authentication process so that the user is correctly logged out on gov login.

## Stub Authentication

It is also possible to enable stub authentication to be used in place of Gov One Login. This is done by adding the `"StubAuth":true` config variable which will then expect a local authentication endpoint
to be setup. The stub authentication service expects a user id and email address and it simply replaces the population of id and email from the Gov One Login. The Id and Email are then used as they would
be in your service and the `GetClaims` method on the `ICustomClaims` interface is called to get any data for populating the claims. An example of implementing a service with StubAuth enabled can be seen at: 
https://github.com/SkillsFundingAgency/das-shared-packages/blob/master/SFA.DAS.GovUk.Auth.Samples/SFA.DAS.GovUK.SampleSite

When the `RequestedUserInfoClaims` are present in the extended configuration the sample site will prompt for a JSON file to be uploaded during a login which uses the StubAuth, the files in 
https://github.com/SkillsFundingAgency/das-shared-packages/blob/master/SFA.DAS.GovUk.Auth.Samples/SFA.DAS.GovUK.SampleSite/Verify are example JSON files which can be uploaded.

To sign out the `SignOut` action result should be called, passing the `CookieAuthenticationDefaults.AuthenticationScheme` as the authentication scheme to sign out from. If using the redis cache session store
this will end the session.


