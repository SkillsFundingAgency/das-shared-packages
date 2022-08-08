namespace SFA.DAS.GovUK.Auth.Interfaces;

internal interface IAzureIdentityService
{
    Task<string> AuthenticationCallback(string authority, string resource, string scope);
}