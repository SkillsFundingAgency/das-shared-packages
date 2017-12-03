namespace SFA.DAS.OidcMiddleware.Validators
{
    public interface ITokenValidator
    {
        void ValidateToken(OidcMiddlewareOptions options, string token, string nonce);
    }
}