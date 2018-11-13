using System;

namespace SFA.DAS.CosmosDb
{
    public interface IDocument
    {
        Guid Id { get; set; }
        string ETag { get; set; }
    }
}
