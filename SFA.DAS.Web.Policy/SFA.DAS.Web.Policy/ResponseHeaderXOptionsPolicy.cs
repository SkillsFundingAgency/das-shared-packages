using System.Collections.Generic;
using System.Web;

namespace SFA.DAS.Web.Policy
{
    /// <summary>
    /// Adds X options headers to an outgoing response
    /// </summary>
    public class ResponseHeaderXOptionsPolicy : IHttpResponsePolicy
    {
        private readonly Dictionary<string,string> _headersToAdd = new Dictionary<string, string>()
        {
            {"X-Frame-Options","deny"},
            {"X-Content-Type-Options","nosniff"},
            {"X-XSS-Protection","1"},
        };

        public void Apply(HttpContextBase context)
        {
            foreach (var header in _headersToAdd)
                context?
                    .Response?
                    .Headers?
                    .Add(header.Key, header.Value);
        }

        public PolicyConcern Concerns { get; } = PolicyConcern.HttpResponse;
    }
}