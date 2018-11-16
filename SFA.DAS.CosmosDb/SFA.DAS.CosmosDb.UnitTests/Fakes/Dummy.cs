using System;

namespace SFA.DAS.CosmosDb.UnitTests.Fakes
{
    public class Dummy : IDocument
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ETag { get; set; }
    }
}