using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using System;


namespace SFA.DAS.CommitmentsApiSubstitute.WebAPI
{
    public class CommitmentsApiMessageHandler : ApiMessageHandlers
    {
        private IObjectCreator _objectCreator;

        public string GetProviderApprenticeship => $"api/provider/{ProviderId}/apprenticeships/{ApprenticeshipId}";

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
            SetupPut(GetProviderApprenticeship);
            SetupGet(GetProviderApprenticeship, response);
        }

        public void ConfigureGetProviderApprenticeship()
        {
            var apprenticeship = _objectCreator.Create<Apprenticeship>(x => { x.ProviderId = ProviderId; x.Id = ApprenticeshipId; });
            SetupGet(GetProviderApprenticeship, apprenticeship);
        }
    }
}
