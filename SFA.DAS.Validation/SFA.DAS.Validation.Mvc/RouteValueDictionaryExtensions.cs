#if NET462
using System.Collections.Specialized;
using System.Web.Routing;

namespace SFA.DAS.Validation.Mvc
{
    public static class RouteValueDictionaryExtensions
    {
        public static void Merge(this RouteValueDictionary routeValues, NameValueCollection queryString)
        {
            foreach (string key in queryString.Keys)
            {
                routeValues[key] = queryString[key];
            }
        }
    }
}
#endif