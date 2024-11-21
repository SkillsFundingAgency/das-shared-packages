namespace SFA.DAS.GovUK.Auth.Models;

public class AuthRedirects
{
    public string SignedOutRedirectUrl { get; set; } = "";
    public string LocalStubLoginPath { get; set; } = "";
    public string CookieDomain { get; set; } = "";
    public string LoginRedirect { get; set; } = "";
}