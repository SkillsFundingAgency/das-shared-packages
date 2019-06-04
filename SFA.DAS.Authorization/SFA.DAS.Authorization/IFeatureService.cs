namespace SFA.DAS.Authorization
{
    public interface IFeatureService
    {
        Feature GetFeature(FeatureType featureType);
    }
}