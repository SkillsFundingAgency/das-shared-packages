using System.Text.Json.Serialization;

namespace SFA.DAS.GovUK.Auth.Models
{
    public class Token
    {
        [JsonPropertyName("access_token")] 
        public string AccessToken { get; set; }

        [JsonPropertyName("id_token")] 
        public string IdToken { get; set; }

        [JsonPropertyName("token_type")] 
        public string TokenType { get; set; }
    }
}