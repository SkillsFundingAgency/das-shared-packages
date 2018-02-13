using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.Provider.Events.Api.Types;
using System.Net;

namespace SFA.DAS.ProviderEventsApiSubstitute.WebAPI
{
    public class ProviderEventsApiMessageHandler : ApiMessageHandlers
    {
        public string DefaultGetSubmissionEventsEndPoint { get; private set; }

        private IObjectCreator _objectCreator;

        public const long ApprenticeshipId = 45785214;
        public const long Ukprn = 10000254;

        public ProviderEventsApiMessageHandler(string baseAddress) : base(baseAddress)
        {
            _objectCreator = new ObjectCreator();
            DefaultGetSubmissionEventsEndPoint = GetSubmissionEventsEndPoint(1);
            ConfigureDefaultResponse();
        }

        private void ConfigureDefaultResponse()
        {
            ConfigureGetSubmissionEvents();
        }

        public void OverrideGetSubmissionEvents<T>(T response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupCall(DefaultGetSubmissionEventsEndPoint, httpStatusCode, response);
        }

        public void SetupGetSubmissionEvents<T>(int page, T response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupCall(GetSubmissionEventsEndPoint(page), httpStatusCode, response);
        }

        private void ConfigureGetSubmissionEvents()
        {
            var submissionEvents = _objectCreator.Create<SubmissionEvent>(x => { x.ApprenticeshipId = ApprenticeshipId ; x.Ukprn = Ukprn; });

            SetupGet(DefaultGetSubmissionEventsEndPoint, new PageOfResults<SubmissionEvent> { Items = new[] { submissionEvents }, PageNumber = 1, TotalNumberOfPages = 1 });
        }
        private string GetSubmissionEventsEndPoint(int page)
        {
            return $"api/submissions?page={page}";
        }
    }
}
