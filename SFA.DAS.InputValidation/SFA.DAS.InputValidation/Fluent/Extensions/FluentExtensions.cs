using FluentValidation;
using SFA.DAS.InputValidation.Fluent.Customisations;

namespace SFA.DAS.InputValidation.Fluent.Extensions
{
    public static class FluentExtensions
    {
        public static IRuleBuilderOptions<T, string?> ValidFreeTextCharacters<T>(
            this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new FreeTextValidator<T, string?>());
        }
    }
}