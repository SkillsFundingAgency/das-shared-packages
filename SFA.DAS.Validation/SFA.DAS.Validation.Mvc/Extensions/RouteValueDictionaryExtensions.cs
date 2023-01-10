using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.Validation.Mvc.Extensions
{
    public static class RouteValueDictionaryExtensions
    {
        public static void Merge(this RouteValueDictionary routeValues, IQueryCollection queryString)
        {
            foreach (var key in queryString.Keys)
            {
                routeValues[key] = queryString[key];
            }
        }
    }
}
