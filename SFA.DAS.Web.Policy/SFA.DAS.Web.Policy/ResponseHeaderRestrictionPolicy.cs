﻿using System.Web;

namespace SFA.DAS.Web.Policy
{
    /// <summary>
    /// Removes some recommended X headers and the Server header from an outgoing http response
    /// </summary>
    public class ResponseHeaderRestrictionPolicy : IHttpResponsePolicy
    {
        private readonly string[] _headersToRemove =
        {
            "X-Powered-By",
            "X-AspNet-Version",
            "X-AspNetMvc-Version",
            "Server"
        };

        public void Apply(HttpContextBase context)
        {
            foreach (var header in _headersToRemove)
                context?
                    .Response?
                        .Headers?
                            .Remove(header);
        }

        public PolicyConcern Concerns { get; } = PolicyConcern.HttpResponse;
    }
}