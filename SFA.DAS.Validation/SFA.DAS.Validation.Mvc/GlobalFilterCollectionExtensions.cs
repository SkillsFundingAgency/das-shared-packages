﻿#if NET462
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    public static class GlobalFilterCollectionExtensions
    {
        public static void AddHttpNotFoundForNullModelFilter(this GlobalFilterCollection filters)
        {
            filters.Add(new HttpNotFoundForNullModelFilter());
        }

        public static void AddValidationFilter(this GlobalFilterCollection filters)
        {
            filters.Add(new ValidateModelStateFilter());
        }
    }
}
#endif