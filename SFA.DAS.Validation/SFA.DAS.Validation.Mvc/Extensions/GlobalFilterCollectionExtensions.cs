#if NET462
using System.Web.Mvc;
using SFA.DAS.Validation.Mvc.Filters;

namespace SFA.DAS.Validation.Mvc.Extensions
{
    public static class GlobalFilterCollectionExtensions
    {
        public static void AddValidationFilter(this GlobalFilterCollection filters)
        {
            filters.Add(new ValidateModelStateFilter());
        }
    }
}
#endif