using System;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Authorization
{
    public class FeatureEnabledAuthorisationHandler : IAuthorizationHandler
    {
        private readonly ILog _logger;
        private static readonly Type Type = typeof(FeatureEnabledAuthorisationHandler);

        public FeatureEnabledAuthorisationHandler(ILog logger)
        {
            _logger = logger;
        }

        public Task<AuthorizationResult> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature)
        {
            _logger.Debug($"Started running '{Type.Name}' for feature '{feature.FeatureType}'");

            var result = feature.Enabled
                ? AuthorizationResult.Ok
                : AuthorizationResult.FeatureDisabled;

            _logger.Debug($"Finished running '{Type.Name}' for feature '{feature.FeatureType}' with result '{result}'");

            return Task.FromResult(result);
        }
    }
}