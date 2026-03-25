using System.Text.Json.Serialization;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IPutApiRequest : IPutApiRequest<object>
    {

    }

    public interface IPutApiRequest<TData> : IBaseApiRequest
    {
        [JsonIgnore]
        string PutUrl { get; }
        TData? Data { get; set; }
    }
}