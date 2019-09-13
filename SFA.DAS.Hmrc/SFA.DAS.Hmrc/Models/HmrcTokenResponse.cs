using Newtonsoft.Json;

namespace SFA.DAS.Hmrc.Models
{
    public class HmrcTokenResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; }

        [JsonProperty("refresh_token")] public string RefreshToken { get; set; }

        [JsonProperty("expires_in")] public int ExpiresIn { get; set; }

        [JsonProperty("scope")] public string Scope { get; set; }

        [JsonProperty("token_type")] public string TokenType { get; set; }
    }
}