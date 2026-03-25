using System.Text.Json.Serialization;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IDeleteApiRequest : IBaseApiRequest
    {
        [JsonIgnore]
        string DeleteUrl { get; }
    }
}