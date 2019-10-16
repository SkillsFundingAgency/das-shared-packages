using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security.Jwt;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Threading;

namespace SFA.DAS.Authentication.Extensions
{
    // This class is necessary because the OAuthBearer Middleware does not leverage
    // the OpenID Connect metadata endpoint exposed by the STS by default.
    // see https://github.com/AzureADQuickStarts/AppModelv2-NativeClient-DotNet/blob/complete/TodoListService/App_Start/OpenIdConnectSecurityTokenProvider.cs

    internal class OpenIdConnectSecurityTokenProvider : IIssuerSecurityTokenProvider
    {
        public ConfigurationManager<OpenIdConnectConfiguration> ConfigManager;
        private string _issuer;
        private IEnumerable<SecurityToken> _tokens;
        private readonly string _metadataEndpoint;
        private readonly ILog _logger;

        private readonly ReaderWriterLockSlim _synclock = new ReaderWriterLockSlim();

        public OpenIdConnectSecurityTokenProvider(string metadataEndpoint, ILog logger)
        {
            _metadataEndpoint = metadataEndpoint;
            ConfigManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataEndpoint);
            _logger = logger;

            RetrieveMetadata();
        }

        /// <summary>
        /// Gets the issuer the credentials are for.
        /// </summary>
        /// <value>
        /// The issuer the credentials are for.
        /// </value>
        public string Issuer
        {
            get
            {
                RetrieveMetadata();
                _synclock.EnterReadLock();
                try
                {
                    return _issuer;
                }
                finally
                {
                    _synclock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets all known security tokens.
        /// </summary>
        /// <value>
        /// All known security tokens.
        /// </value>
        public IEnumerable<SecurityToken> SecurityTokens
        {
            get
            {
                RetrieveMetadata();
                _synclock.EnterReadLock();
                try
                {
                    return _tokens;
                }
                finally
                {
                    _synclock.ExitReadLock();
                }
            }
        }

        private void RetrieveMetadata()
        {
            _synclock.EnterWriteLock();
            try
            {
                OpenIdConnectConfiguration config = ConfigManager.GetConfigurationAsync().Result;
                _issuer = config.Issuer;
                _tokens = config.SigningTokens;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve Issuer and SigningTokens from the provided Metadata endpoint in the config. Check the Metadata endpoint in the config is available");
                throw;
            }
            finally
            {
                _synclock.ExitWriteLock();
            }
        }
    }
}
