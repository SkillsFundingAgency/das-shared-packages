using System.Linq;

namespace SFA.DAS.Learners.Validators
{
    public class UlnValidator : IUlnValidator
    {
        public bool Validate(long uln)
        {
            if (uln < 0)
            {
                return false;
            }

            if (uln.ToString().Length != 10)
            {
                return false;
            }

            var ulnCheckArray = uln.ToString()
                                    .ToCharArray()
                                    .Select(c => int.Parse(c.ToString()))
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
