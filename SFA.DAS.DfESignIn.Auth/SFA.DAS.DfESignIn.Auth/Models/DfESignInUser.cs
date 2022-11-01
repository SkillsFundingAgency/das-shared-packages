using System.Text.Json.Serialization;

namespace SFA.DAS.DfESignIn.Auth.Models
{
    public class DfESignInUser
    {
        [JsonPropertyName("sub")] 
        public string Sub { get; set; }

        [JsonPropertyName("phone_number_verified")]
        public bool PhoneNumberVerified { get; set; }

        [JsonPropertyName("phone_number")] 
        public string PhoneNumber { get; set; }

        [JsonPropertyName("email_verified")] 
        public bool EmailVerified { get; set; }

        [JsonPropertyName("email")] 
        public string Email { get; set; }

        [JsonPropertyName("ukprn")]
        public int? Ukprn { get; set; }


    }
}