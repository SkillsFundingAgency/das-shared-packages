using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.Commitments.Api.Client
{
    public class RelationshipApi : HttpClientBase, IRelationshipApi
    {
        private readonly ICommitmentsApiClientConfiguration _configuration;

        public RelationshipApi(ICommitmentsApiClientConfiguration configuration)
            : base(configuration.ClientToken)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
        }

        public async Task<Relationship> GetRelationship(long providerId, long employerAccountId, string legalEntityId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/relationships/{employerAccountId}/{legalEntityId}";
            return await GetRelationship(url);
        }

        public async Task PatchRelationship(long providerId, long employerAccountId, string legalEntityId, RelationshipRequest relationshipRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/relationships/{employerAccountId}/{legalEntityId}";
            await PatchRelationship(url, relationshipRequest);
        }

        public async Task<Relationship> GetRelationshipByCommitment(long providerId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/relationships/{commitmentId}";
            return await GetRelationship(url);
        }

        private async Task<Relationship> GetRelationship(string url)
        {
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<Relationship>(content);
        }

        private async Task PatchRelationship(string url, RelationshipRequest relationshipRequest)
        {
            var data = JsonConvert.SerializeObject(relationshipRequest);
            await PatchAsync(url, data);
        }
    }
}