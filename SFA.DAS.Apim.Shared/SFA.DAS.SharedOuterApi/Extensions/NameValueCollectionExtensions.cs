using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace SFA.DAS.SharedOuterApi.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static string ToQueryString(this NameValueCollection source)
        {
            if (source.Count == 0) return string.Empty;

            var list = new List<string>();
            foreach (var key in source.AllKeys)
            {
                foreach (var value in source.GetValues(key)!)
                {
                    list.Add($"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}");
                }
            }

            return "?" + string.Join("&", list);
        }
    }
}
