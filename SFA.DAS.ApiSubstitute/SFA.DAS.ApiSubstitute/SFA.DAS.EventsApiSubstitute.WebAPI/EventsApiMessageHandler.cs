using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using System.Net;

namespace SFA.DAS.EventsApiSubstitute.WebAPI
{
    public class EventsApiMessageHandler : ApiMessageHandlers
    {
        public string DefaultCreateGenericEventEndPoint { get; set; }

        private IObjectCreator _objectCreator;

        public EventsApiMessageHandler(string baseAddress) : base(baseAddress)
        {
            _objectCreator = new ObjectCreator();
            ConfigureDefaultResponse();
        }

        private void ConfigureDefaultResponse()
        {
            ConfigureCreateGenericEvent();
        }

        public void OverrideCreateGenericEvent<T>(T response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupCall(DefaultCreateGenericEventEndPoint, httpStatusCode, response);
        }

        public void SetupCreateGenericEvent<T>(T response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupCall(CreateGenericEvent(), httpStatusCode, response);
        }

        private void ConfigureCreateGenericEvent()
        {
            DefaultCreateGenericEventEndPoint = CreateGenericEvent();

            SetupGet(DefaultCreateGenericEventEndPoint, HttpStatusCode.OK, string.Empty);
        }

        private string CreateGenericEvent()
        {
            return $"/api/events/create";
        }
    }
}