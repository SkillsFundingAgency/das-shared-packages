using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Http
{
    internal sealed class QueryStringHelper
    {
        public string GetQueryString(object obj)
        {
            if (obj == null)
                return string.Empty;

            var result = new List<string>();
            var props = obj.GetType().GetProperties().Where(p => p.GetValue(obj, null) != null);
            foreach (var p in props)
            {
                var value = p.GetValue(obj, null);
                var enumerable = value as ICollection;
                if (enumerable != null)
                {
                    result.AddRange(from object v in enumerable select $"{p.Name}={v}");
                }
                else
                {
                    result.Add($"{p.Name}={value}");
                }
            }

            return result.Any()
                ? "?" + string.Join("&", result.ToArray())
                : string.Empty;
        }
    }
}
