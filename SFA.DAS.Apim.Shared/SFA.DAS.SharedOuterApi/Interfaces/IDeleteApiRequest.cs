using System.Text.Json.Serialization;

namespace SFA.DAS.SharedOuterApi.Interfaces
{
    public interface IDeleteApiRequest : IBaseApiRequest
    {
        [JsonIgnore]
        string DeleteUrl { get; }
    }
}