using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SFA.DAS.Authentication.Extensions.Legacy
{
    internal sealed class QueryStringHelper
    {
        public string GetQueryString(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            List<string> list = new List<string>();
            foreach (PropertyInfo p2 in from p in obj.GetType().GetProperties()
                                        where p.GetValue(obj, null) != null
                                        select p)
            {
                object value = p2.GetValue(obj, null);
                ICollection collection = value as ICollection;
                if (collection != null)
                {
                    list.AddRange(from object v in collection
                                  select $"{p2.Name}={v}");
                }
                else
                {
                    list.Add($"{p2.Name}={value}");
                }
            }
            if (!list.Any())
            {
                return string.Empty;
            }
            return "?" + string.Join("&", list.ToArray());
        }
    }
}
