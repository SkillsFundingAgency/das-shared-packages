namespace SFA.DAS.GovUK.Auth.Interfaces;

public interface IAzureIdentityService
{
    Task<string> AuthenticationCallback(string authority, string resource, string scope);
}