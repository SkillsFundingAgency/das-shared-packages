using FluentValidation;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.SampleSite.Models;

namespace SFA.DAS.GovUK.SampleSite.Validators
{
    public class SignInStubViewModelValidator : AbstractValidator<SignInStubViewModel>
    {
        public SignInStubViewModelValidator(GovUkOidcConfiguration config)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Enter a valid email address");

            When(_ => !string.IsNullOrWhiteSpace(config.RequestedUserInfoClaims), () =>
            {
                RuleFor(x => x.UserFile)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage("You must upload a JSON file with verified identity information")
                    .Must(file => file != null && file.Length > 0)
                    .WithMessage("The uploaded file must not be empty")
                    .Must(file => file!.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Only JSON files can be uploaded");
            });
        }
    }
}
