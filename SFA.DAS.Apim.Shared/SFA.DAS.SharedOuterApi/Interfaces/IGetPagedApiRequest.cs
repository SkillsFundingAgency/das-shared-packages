using System.Text.Json.Serialization;

namespace SFA.DAS.SharedOuterApi.Interfaces
{
    public interface IGetPagedApiRequest : IBaseApiRequest
    {
        [JsonIgnore]
        string GetPagedUrl { get; }
        [JsonIgnore]
        int PageNumber { get; }
        [JsonIgnore]
        int PageSize { get; }
    }
}