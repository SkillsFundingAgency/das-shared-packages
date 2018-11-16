using System;

namespace SFA.DAS.CosmosDb
{
    public interface IDocument
    {
        Guid Id { get; }
        string ETag { get; }
    }
}
