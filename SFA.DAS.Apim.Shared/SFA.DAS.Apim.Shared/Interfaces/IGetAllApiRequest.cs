using System.Text.Json.Serialization;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IGetAllApiRequest : IBaseApiRequest
    {
        [JsonIgnore]
        string GetAllUrl { get; }
    }
}