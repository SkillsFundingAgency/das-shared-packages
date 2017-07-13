﻿namespace SFA.DAS.Http
{
    public interface IAzureADClientConfiguration
    {
        // Azure AD Configuration
        string Tenant { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string IdentifierUri { get; }
    }
}
