using System.Text.Json.Serialization;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IHeadApiRequest : IBaseApiRequest
    {
        [JsonIgnore]
        string HeadUrl { get; }
    }
}
