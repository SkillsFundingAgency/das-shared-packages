using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Validation.NetCoreSample.ViewModels.Accounts
{
    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "The 'Username' field is required")]
        public string Username { get; set; }
    }
}