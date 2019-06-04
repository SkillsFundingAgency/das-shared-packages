namespace SFA.DAS.Authorization
{
    public enum AuthorizationResult
    {
        Ok,
        FeatureDisabled,
        FeatureAgreementNotSigned,
        FeatureUserNotWhitelisted
    }
}