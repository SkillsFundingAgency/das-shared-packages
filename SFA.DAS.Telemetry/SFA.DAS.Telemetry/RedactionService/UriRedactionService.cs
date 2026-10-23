using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SFA.DAS.Telemetry.RedactionService
{
    public class UriRedactionService : IUriRedactionService
    {
        private readonly string _redactionString;
        private readonly HashSet<string> _keysToRedact;

        public UriRedactionService(UriRedactionOptions options)
        {
            _redactionString = options.RedactionString;
            _keysToRedact = options.RedactionList
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public Uri GetRedactedUri(Uri uri)
        {
            if (string.IsNullOrEmpty(uri.Query) || uri.Query == "?*")
            {
                // the default MS redactor in .Net 9+ will reduce the query to ?*
                return uri;
            }

            var components = HttpUtility.ParseQueryString(uri.Query);

            var redactionList = components.AllKeys
                .Where(key => !string.IsNullOrWhiteSpace(key) && _keysToRedact.Contains(key))
                .ToList();

            foreach (var redaction in redactionList)
            {
                components[redaction] = _redactionString;
            }

            var uriBuilder = new UriBuilder(uri)
            {
                Query = components.ToString()
            };

            var newUri = uriBuilder.Uri;
            return newUri;
        }

        public string GetRedactedString(string input)
        {
            Regex redactionRegex = new Regex(
                $@"(?i)\b({string.Join("|", _keysToRedact)})=([^&\s]+)",
                RegexOptions.Compiled, TimeSpan.FromSeconds(30));

            return redactionRegex.Replace(input, $"$1={_redactionString}");
        }
    }
}