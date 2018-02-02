using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.ProviderEventsApiSubstitute.WebAPI
{
    public class ProviderEventsApiMessageHandler : ApiMessageHandlers
    {
        private IObjectCreator _objectCreator;

        public string GetSubmissionEventsEndPoint => "api/submissions?page=1";

        public const long ApprenticeshipId = 45785214;
        public const long Ukprn = 10000254;

        public ProviderEventsApiMessageHandler(string baseAddress) : base(baseAddress)
        {
            _objectCreator = new ObjectCreator();
            ConfigureDefaultResponse();
        }

        public void ConfigureDefaultResponse()
        {
            ConfigureGetSubmissionEvents();
        }

        public void OverrideGetSubmissionEvents<T>(T response)
        {
            SetupPut(GetSubmissionEventsEndPoint);
            SetupGet(GetSubmissionEventsEndPoint, response);
        }

        private void ConfigureGetSubmissionEvents()
        {
            var submissionEvents = _objectCreator.Create<SubmissionEvent>(x => { x.ApprenticeshipId = ApprenticeshipId ; x.Ukprn = Ukprn; });

            SetupGet(GetSubmissionEventsEndPoint, new PageOfResults<SubmissionEvent> { Items = new[] { submissionEvents }, PageNumber = 1, TotalNumberOfPages = 1 });
        }
    }
}
