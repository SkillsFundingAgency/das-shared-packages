using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.Telemetry.RedactionService
{
    public class UriRedactionService : IUriRedactionService
    {
        private readonly UriRedactionOptions _options;

        public UriRedactionService(UriRedactionOptions options)
        {
            _options = options;
        }

        public Uri GetRedactedUri(Uri uri)
        {
            var components = HttpUtility.ParseQueryString(uri.Query);

            var redactionList = components.AllKeys.Where(key => _options.RedactionList.Contains(key)).ToList();

            foreach (var redaction in redactionList)
            {
                components[redaction] = _options.RedactionString;
            }

            var uriBuilder = new UriBuilder(uri)
            {
                Query = components.ToString()
            };

            var newUri = uriBuilder.Uri;
            return newUri;
        }
    }
}