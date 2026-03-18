using System.Text.Json.Serialization;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IPostApiRequest : IPostApiRequest<object>
    {
    }

    public interface IPostApiRequest<TData> : IBaseApiRequest
    {
        [JsonIgnore]
        string PostUrl { get; }
        TData Data { get; set; }
    }
}