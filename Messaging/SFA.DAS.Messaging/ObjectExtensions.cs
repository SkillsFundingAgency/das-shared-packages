using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public static class ObjectExtensions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypeCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>
        ///     Converts the public properties on the supplied object to a case-insensitive dictionary.
        /// </summary>
        /// <remarks>
        ///     This allows arbitrary objects to be provided to the logger (using the overloads that accept an IIDictionary)
        /// </remarks>
        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            var properties = TypeCache.GetOrAdd(obj.GetType(), type => type.GetProperties());

            return properties
                .Select(propertyInfo => new
                {
                    Name = propertyInfo.Name,
                    Value = propertyInfo.GetValue(obj, null)
                })
                .ToDictionary(ks => ks.Name, vs => vs.Value, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
