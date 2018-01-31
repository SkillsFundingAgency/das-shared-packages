using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SFA.DAS.NLog.Targets.Redis.DotNetCore
{
    // Class taken from https://github.com/ReactiveMarkets/NLog.Targets.ElasticSearch/commit/ca8e5888436d8ccf3168430043849630c5cbd7f5
    public static class ExpandoObjectExtensions
    {
        /// <summary>
        /// Replaces dot ('.') character in Keys with an underscore ('_') 
        /// </summary>
        /// <returns>ExpandoObject</returns>
        public static ExpandoObject ReplaceDotInKeys(this ExpandoObject obj)
        {
            var clone = new ExpandoObject();
            foreach (var item in obj)
            {
                if (item.Value == null)
                    continue;

                if (item.Value.GetType() == typeof(ExpandoObject))
                    ((IDictionary<string, object>)clone)[item.Key.Replace('.', '_')]
                        = (item.Value as ExpandoObject).ReplaceDotInKeys();
                else if (item.Key.Contains('.'))
                    ((IDictionary<string, object>)clone)[item.Key.Replace('.', '_')] = item.Value;
                else
                    ((IDictionary<string, object>)clone)[item.Key] = item.Value;
            }
            return clone;
        }
    }
}