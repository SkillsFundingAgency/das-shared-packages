using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Learners.Validators
{
    public enum UlnValidationResult
    {
        Success,
        IsEmptyUlnNumber,
        IsInValidTenDigitUlnNumber,
        IsInvalidUln
    }
}
