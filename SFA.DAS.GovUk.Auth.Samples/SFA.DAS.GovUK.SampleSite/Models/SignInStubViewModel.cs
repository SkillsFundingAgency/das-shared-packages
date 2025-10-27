namespace SFA.DAS.GovUK.SampleSite.Models
{
    public class SignInStubViewModel
    {
        public string? Id { get; set;  }
        public string? Email { get; set; }

        public IFormFile? UserFile { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
