using System.Text.Json.Serialization;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IPatchApiRequest<TData> : IBaseApiRequest
    {
        [JsonIgnore]
        string PatchUrl { get; }

        TData Data { get; set; }
    }
}