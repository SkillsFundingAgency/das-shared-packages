using System.Text.Json.Serialization;

namespace SFA.DAS.Authorization.TokenGenerator.Models;

public class ServiceAccount
{
    [JsonPropertyName("sub")]
    public string? Sub { get; set; }

    //[JsonPropertyName("serviceAccount")]
    //public string? ServiceAccountId { get; set; }
}
