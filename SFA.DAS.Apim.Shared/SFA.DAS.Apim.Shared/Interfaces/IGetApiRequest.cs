using System.Text.Json.Serialization;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IGetApiRequest : IBaseApiRequest
    {
        [JsonIgnore]
        string GetUrl { get; }
    }
}
