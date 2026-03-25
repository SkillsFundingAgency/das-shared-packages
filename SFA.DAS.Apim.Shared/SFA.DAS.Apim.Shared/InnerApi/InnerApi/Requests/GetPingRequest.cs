using SFA.DAS.Apim.Shared.Interfaces;

namespace SFA.DAS.Apim.Shared.InnerApi.InnerApi.Requests
{
    public class GetPingRequest : IGetApiRequest
    {
        public string GetUrl => "ping";
    }
}