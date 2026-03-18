using System.Collections.Generic;
using System.Collections.Specialized;

namespace SFA.DAS.SharedOuterApi.Extensions
{
    public static class EnumerableStringExtensions
    {
        public static NameValueCollection ToNameValueCollection(this IEnumerable<string> value, string key)
        {
            var result = new NameValueCollection();
            foreach (var sector in value)
            {
                result.Add(key, sector);
            }

            return result;
        }
    }
}
