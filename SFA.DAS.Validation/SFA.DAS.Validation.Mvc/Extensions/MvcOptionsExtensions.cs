using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Validation.Mvc.Filters;

namespace SFA.DAS.Validation.Mvc.Extensions
{
    public static class MvcOptionsExtensions
    {
        public static void AddValidation(this MvcOptions mvcOptions)
        {
            mvcOptions.Filters.Add<ValidateModelStateFilter>(int.MaxValue);
        }
    }
}
