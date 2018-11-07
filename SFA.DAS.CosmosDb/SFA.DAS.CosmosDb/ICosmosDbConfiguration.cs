namespace SFA.DAS.CosmosDb
{
    public interface ICosmosDbConfiguration
    {
        string Uri { get; set; }
        string AuthKey { get; set; }
    }
}