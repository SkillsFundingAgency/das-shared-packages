using System;
using System.Net.Http;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface IDfESignInClient
    {
        HttpClient HttpClient { get; }
        string OrganisationId { get; set; }
        string ServiceId { get; set; }
        string ServiceUrl { get; set; }
        Uri TargetAddress { get; }
        string UserId { get; set; }
    }
}