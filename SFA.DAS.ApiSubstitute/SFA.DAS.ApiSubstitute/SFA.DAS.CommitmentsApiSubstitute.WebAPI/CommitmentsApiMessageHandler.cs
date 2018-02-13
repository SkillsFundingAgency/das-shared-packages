using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using System.Net;

namespace SFA.DAS.CommitmentsApiSubstitute.WebAPI
{
    public class CommitmentsApiMessageHandler : ApiMessageHandlers
    {

        public string DefaultGetProviderApprenticeshipEndPoint { get; private set; }

        private IObjectCreator _objectCreator;

        public const long ProviderId = 10002457;
        public const long ApprenticeshipId = 45784125;

        public CommitmentsApiMessageHandler(string baseAddress) : base(baseAddress)
        {
            _objectCreator = new ObjectCreator();
            ConfigureDefaultResponse();
        }

        public void ConfigureDefaultResponse()
        {
            ConfigureGetProviderApprenticeship();
        }

        public void OverrideGetProviderApprenticeship<T>(T response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupPatch(DefaultGetProviderApprenticeshipEndPoint, httpStatusCode, response);
        }

        public void SetupGetProviderApprenticeship<T>(long providerId, long apprenticeshipId, T response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupPatch(GetProviderApprenticeship(providerId, apprenticeshipId), httpStatusCode, response);
        }

        public void ConfigureGetProviderApprenticeship()
        {
            var apprenticeship = _objectCreator.Create<Apprenticeship>(x => { x.ProviderId = ProviderId; x.Id = ApprenticeshipId; });

            DefaultGetProviderApprenticeshipEndPoint = GetProviderApprenticeship(ProviderId, ApprenticeshipId);

            SetupGet(DefaultGetProviderApprenticeshipEndPoint, apprenticeship);
        }
        private string GetProviderApprenticeship(long providerId, long apprenticeshipId)
        {
            return $"api/provider/{providerId}/apprenticeships/{apprenticeshipId}";
        }
    }
}
