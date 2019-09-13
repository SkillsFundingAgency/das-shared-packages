using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using HMRC.ESFA.Levy.Api.Client;
using HMRC.ESFA.Levy.Api.Types;
using Newtonsoft.Json;
using SFA.DAS.ActiveDirectory;
using SFA.DAS.Caches;
using SFA.DAS.Hrmc.Configuration;
using SFA.DAS.Hrmc.ExecutionPolicy;
using SFA.DAS.Hrmc.Http;
using SFA.DAS.Hrmc.Models;
using SFA.DAS.NLog.Logger;
using SFA.DAS.TokenService.Api.Client;

namespace SFA.DAS.Hrmc
{
    public class HmrcService : IHmrcService
    {
        private readonly IApprenticeshipLevyApiClient _apprenticeshipLevyApiClient;
        private readonly IAzureAdAuthenticationService _azureAdAuthenticationService;
        private readonly IHmrcConfiguration _configuration;
        private readonly ExecutionPolicy.ExecutionPolicy _executionPolicy;
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly IInProcessCache _inProcessCache;
        private readonly ILog _log;
        private readonly ITokenServiceApiClient _tokenServiceApiClient;


        public HmrcService(
            IHmrcConfiguration configuration,
            IHttpClientWrapper httpClientWrapper,
            IApprenticeshipLevyApiClient apprenticeshipLevyApiClient,
            ITokenServiceApiClient tokenServiceApiClient,
            [RequiredPolicy(HmrcExecutionPolicy.Name)]
            ExecutionPolicy.ExecutionPolicy executionPolicy,
            IInProcessCache inProcessCache,
            IAzureAdAuthenticationService azureAdAuthenticationService,
            ILog log)
        {
            _configuration = configuration;
            _httpClientWrapper = httpClientWrapper;
            _apprenticeshipLevyApiClient = apprenticeshipLevyApiClient;
            _tokenServiceApiClient = tokenServiceApiClient;
            _executionPolicy = executionPolicy;
            _inProcessCache = inProcessCache;
            _azureAdAuthenticationService = azureAdAuthenticationService;
            _log = log;

            _httpClientWrapper.BaseUrl = _configuration.BaseUrl;
            _httpClientWrapper.AuthScheme = "Bearer";
            _httpClientWrapper.MediaTypeWithQualityHeaderValueList = new List<MediaTypeWithQualityHeaderValue> { new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json") };
        }

        public string GenerateAuthRedirectUrl(string redirectUrl)
        {
            var urlFriendlyRedirectUrl = HttpUtility.UrlEncode(redirectUrl);
            return $"{_configuration.BaseUrl}oauth/authorize?response_type=code&client_id={_configuration.ClientId}&scope={_configuration.Scope}&redirect_uri={urlFriendlyRedirectUrl}";
        }

        public async Task<HmrcTokenResponse> GetAuthenticationToken(string redirectUrl, string accessCode)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var requestParams = new
                {
                    client_secret = _configuration.ClientSecret,
                    client_id = _configuration.ClientId,
                    grant_type = "authorization_code",
                    redirect_uri = redirectUrl,
                    code = accessCode
                };

                var response = await _httpClientWrapper.SendMessage(requestParams, "oauth/token");

                return JsonConvert.DeserializeObject<HmrcTokenResponse>(response);
            });
        }

        public async Task<EmpRefLevyInformation> GetEmprefInformation(string authToken, string empRef)
        {
            return await _executionPolicy.ExecuteAsync(async () => await _apprenticeshipLevyApiClient.GetEmployerDetails(authToken, empRef));
        }

        public async Task<EmpRefLevyInformation> GetEmprefInformation(string empRef)
        {
            var accessToken = await _executionPolicy.ExecuteAsync(async () => await GetOgdAccessToken());

            return await GetEmprefInformation(accessToken, empRef);
        }

        public async Task<string> DiscoverEmpref(string authToken)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var response = await _apprenticeshipLevyApiClient.GetAllEmployers(authToken);

                if (response == null)
                    return string.Empty;

                return response.Emprefs.SingleOrDefault();
            });
        }

        public async Task<LevyDeclarations> GetLevyDeclarations(string empRef)
        {
            return await GetLevyDeclarations(empRef, null);
        }

        public async Task<LevyDeclarations> GetLevyDeclarations(string empRef, DateTime? fromDate)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var accessToken = await GetOgdAccessToken();

                var earliestDate = new DateTime(2017, 04, 01);
                if (!fromDate.HasValue || fromDate.Value < earliestDate) fromDate = earliestDate;

                var levyDeclartions = await _apprenticeshipLevyApiClient.GetEmployerLevyDeclarations(accessToken, empRef, fromDate);

                _log.Debug($"Received {levyDeclartions?.Declarations?.Count} levy declarations empRef:{empRef} fromDate:{fromDate}");
                return levyDeclartions;
            });
        }

        public async Task<EnglishFractionDeclarations> GetEnglishFractions(string empRef)
        {
            return await GetEnglishFractions(empRef, null);
        }

        public async Task<EnglishFractionDeclarations> GetEnglishFractions(string empRef, DateTime? fromDate)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                var accessToken = await GetOgdAccessToken();

                return await _apprenticeshipLevyApiClient.GetEmployerFractionCalculations(accessToken, empRef, fromDate);
            });
        }

        public async Task<DateTime> GetLastEnglishFractionUpdate()
        {
            var hmrcLatestUpdateDate = _inProcessCache.Get<DateTime?>("HmrcFractionLastCalculatedDate");
            if (hmrcLatestUpdateDate == null)
                return await _executionPolicy.ExecuteAsync(async () =>
                {
                    var accessToken = await GetOgdAccessToken();

                    hmrcLatestUpdateDate = await _apprenticeshipLevyApiClient.GetLastEnglishFractionUpdate(accessToken);

                    if (hmrcLatestUpdateDate != null) _inProcessCache.Set("HmrcFractionLastCalculatedDate", hmrcLatestUpdateDate.Value, new TimeSpan(0, 0, 30, 0));

                    return hmrcLatestUpdateDate.Value;
                });
            return hmrcLatestUpdateDate.Value;
        }

        public async Task<string> GetOgdAccessToken()
        {
            if (_configuration.UseHiDataFeed)
            {
                var accessToken =
                    await _azureAdAuthenticationService.GetAuthenticationResult(_configuration.ClientId,
                        _configuration.AzureAppKey, _configuration.AzureResourceId,
                        _configuration.AzureTenant);

                return accessToken;
            }
            else
            {
                var accessToken = await _tokenServiceApiClient.GetPrivilegedAccessTokenAsync();
                return accessToken.AccessCode;
            }
        }
    }
}