
using System;
using System.Net.Http;

namespace SFA.DAS.DfESignIn.SampleSite.Classic.Api.Models
{
    public class DfESignInClient
    {
        public DfESignInClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; private set; }

        public string OrganisationId { get; set; }

        public string ServiceId { get; set; }

        public string ServiceUrl { get; set; }

        public string UserId { get; set; }

        public Uri TargetAddress
        {
            get
            {
                if (string.IsNullOrEmpty(ServiceUrl) | string.IsNullOrEmpty(ServiceId) | string.IsNullOrEmpty(OrganisationId) | string.IsNullOrEmpty(UserId))
                {
                    throw new MemberAccessException("Required Member(s) not set");
                }
                return new Uri($"{ServiceUrl}/services/{ServiceId}/organisations/{OrganisationId}/users/{UserId}");
            }
        }
    }
}
