namespace SFA.DAS.GovUK.Auth.Models
{
    public class StubAuthUserDetails
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public GovUkUser GovUkUser { get; set; }
    }    
}
