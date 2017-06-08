using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.Commitments.Api.Client
{
    public class ApprenticeshipApi : HttpClientBase, IApprenticeshipApi
    {
        private readonly ICommitmentsApiClientConfiguration _configuration;

        public ApprenticeshipApi(ICommitmentsApiClientConfiguration configuration)
            : base(configuration.ClientToken)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _configuration = configuration;
        }

        public async Task<PriceHistory> GetPriceHistory(long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/apprenticeships/{apprenticeshipId}/prices";
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<PriceHistory>(content);
        }

    }
}