namespace SFA.DAS.Elastic
{
    public interface IElasticConfiguration
    {
        string ElasticUrl { get; }
        string ElasticUsername { get; }
        string ElasticPassword { get; }
    }
}