using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.GovUK.Auth.Models
{
    [ExcludeFromCodeCoverage]
    public class AuthRedirects
    {
        public string SuspendedRedirectUrl { get; set; } = string.Empty;
        public string SignedOutRedirectUrl { get; set; } = string.Empty;
        public string LoginRedirect { get; set; } = string.Empty; 
        public string LocalStubLoginPath { get; set; } = string.Empty;
        public string CookieDomain { get; set; } = string.Empty;
        public string VerifyIdentityInformationUrl { get; set; } = string.Empty;
    }
}