using System;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

namespace SFA.DAS.InputValidation.Fluent.Customisations
{
    internal class FreeTextValidator<T, TProperty> : PropertyValidator<T, TProperty>, IRegularExpressionValidator
    {
        public override string Name => "FreeTextValidator";

        private Regex _regex = null!;

        private const string ValidCharactersExpression = @"^[a-zA-Z0-9\u0080-\uFFA7?$@#()""'!,+\-=_:;.&€£*%\s\/\[\]]*$";
        
        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "{PropertyName} must only include letters a to z, numbers 0 to 9, and special characters such as hyphens, spaces and apostrophes";
        }

        public override bool IsValid(ValidationContext<T> context, TProperty propertyValue)
        {
            if (propertyValue == null)
            {
                return true;
            }

            _regex = CreateRegEx();
            return _regex.IsMatch(propertyValue as string ?? string.Empty);
        }

        public string Expression => ValidCharactersExpression;

        private static Regex CreateRegEx()
        {
            try
            {
                if (AppDomain.CurrentDomain.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") == null)
                {
                    return new Regex(ValidCharactersExpression, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2.0));
                }
            }
            catch (Exception)
            {
            }

            return new Regex(ValidCharactersExpression, RegexOptions.IgnoreCase);
        }
    }
}