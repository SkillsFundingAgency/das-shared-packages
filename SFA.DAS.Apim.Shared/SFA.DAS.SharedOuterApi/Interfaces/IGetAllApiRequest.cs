using System.Text.Json.Serialization;

namespace SFA.DAS.SharedOuterApi.Interfaces
{
    public interface IGetAllApiRequest : IBaseApiRequest
    {
        [JsonIgnore]
        string GetAllUrl { get; }
    }
}