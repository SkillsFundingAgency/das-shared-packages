using System;

namespace SFA.DAS.Telemetry.RedactionService
{
    public interface IUriRedactionService
    {
        /// <summary>
        /// Returns a Uri with sensitive information redacted
        /// </summary>
        /// <param name="uri">The original URI from which to redact sensitive information</param>
        /// <returns></returns>
        Uri GetRedactedUri(Uri uri);
    }
}