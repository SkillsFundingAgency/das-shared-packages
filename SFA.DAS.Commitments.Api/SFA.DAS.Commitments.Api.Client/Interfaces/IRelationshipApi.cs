using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.Commitments.Api.Client.Interfaces
{
    public interface IRelationshipApi
    {
        Task<Relationship> GetRelationship(long providerId, long employerAccountId, string legalEntityId);
        Task<Relationship> GetRelationshipByCommitment(long providerId, long commitmentId);
        Task PatchRelationship(long providerId, long employerAccountId, string legalEntityId, RelationshipRequest relationshipRequest);
    }
}