using System.Linq;
using System.Text.RegularExpressions;

namespace SFA.DAS.Learners.Validators
{
    public class UlnValidator : IUlnValidator
    {
        public UlnValidationResult Validate(string uln)
        {
            if (string.IsNullOrEmpty(uln))
            {
                return UlnValidationResult.IsEmptyUlnNumber;
            }

            var regex = new Regex("^[1-9]{1}[0-9]{9}$");
            if (!regex.IsMatch(uln))
            {
                return UlnValidationResult.IsInValidTenDigitUlnNumber;
            }

            var ulnNumber = long.Parse(uln);
            if (ulnNumber < 0)
            {
                return UlnValidationResult.IsInValidTenDigitUlnNumber;
            }

            if(!IsValidCheckSum(uln))
            {
                return UlnValidationResult.IsInvalidUln;
            }

            return UlnValidationResult.Success;
        }

        private bool IsValidCheckSum(string uln)
        {
            var ulnCheckArray = uln.ToCharArray()
                                    .Select(c => long.Parse(c.ToString()))
                                    .ToList();

            var multiplier = 10;
            long checkSumValue = 0;
            for (var i = 0; i < 10; i++)
            {
                checkSumValue += ulnCheckArray[i] * multiplier;
                multiplier--;
            }

            return checkSumValue % 11 == 10;
        }
    }
}
