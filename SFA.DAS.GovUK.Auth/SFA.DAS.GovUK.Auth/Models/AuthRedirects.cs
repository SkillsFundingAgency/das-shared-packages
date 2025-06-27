using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.GovUK.Auth.Models
{
    [ExcludeFromCodeCoverage]
    public class AuthRedirects
    {
        public string SignedOutRedirectUrl { get; set; } = "";
        public string LoginRedirect { get; set; } = ""; 
        public string LocalStubLoginPath { get; set; } = "";
        public string CookieDomain { get; set; } = "";
    }
}