using System.Text.Json.Serialization;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IBaseApiRequest 
    {
        [JsonIgnore]
        string Version => "1.0";
    }
}
