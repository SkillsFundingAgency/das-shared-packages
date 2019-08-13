#if NETCOREAPP2_0
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    public static class MvcOptionsExtensions
    {
        public static void AddValidation(this MvcOptions mvcOptions)
        {
            mvcOptions.Filters.Add<ValidateModelStateFilter>(int.MaxValue);
        }
    }
}
#endif