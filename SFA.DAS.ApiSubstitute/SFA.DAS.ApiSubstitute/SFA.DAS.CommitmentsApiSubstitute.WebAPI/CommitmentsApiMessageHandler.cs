using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using System;


namespace SFA.DAS.CommitmentsApiSubstitute.WebAPI
{
    public class CommitmentsApiMessageHandler : ApiMessageHandlers
    {

        public string DefaultGetProviderApprenticeshipEndPoint { get; private set; }

        private IObjectCreator _objectCreator;

        public string GetProviderApprenticeship(long providerId, long apprenticeshipId)
        {
            return $"api/provider/{providerId}/apprenticeships/{apprenticeshipId}";
        }

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

        public void OverrideGetProviderApprenticeship<T>(T response)
        {
            SetupPatch(DefaultGetProviderApprenticeshipEndPoint, response);
        }

        public void SetupGetProviderApprenticeship<T>(long providerId, long apprenticeshipId, T response)
        {
            SetupPatch(GetProviderApprenticeship(providerId, apprenticeshipId), response);
        }


        public void ConfigureGetProviderApprenticeship()
        {
            var apprenticeship = _objectCreator.Create<Apprenticeship>(x => { x.ProviderId = ProviderId; x.Id = ApprenticeshipId; });

            DefaultGetProviderApprenticeshipEndPoint = GetProviderApprenticeship(ProviderId, ApprenticeshipId);

            SetupGet(DefaultGetProviderApprenticeshipEndPoint, apprenticeship);
        }
    }
}
