namespace SFA.DAS.CosmosDb.UnitTests.Fakes
{
    public class DummyConfiguration : ICosmosDbConfiguration
    {
        public string Uri { get; set; }
        public string AuthKey { get; set; }
    }
}