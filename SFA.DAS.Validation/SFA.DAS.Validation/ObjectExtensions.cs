using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SFA.DAS.Validation
{
    public static class ObjectExtensions
    {
        public static string GetPath<T1, T2>(this T1 source, T2 item, string sourceName = "") where T1 : class where T2 : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source is string || source is ValueType)
                throw new ArgumentException("Parameter must be a reference type", nameof(source));

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item is string || item is ValueType)
                throw new ArgumentException("Parameter must be a reference type", nameof(item));

            return source == item ? sourceName : GetPath(source, item, sourceName, source.GetType().GetProperties());
        }

        private static string GetPath<T1, T2>(T1 source, T2 item, string sourceName, IEnumerable<PropertyInfo> sourceProperties) where T1 : class where T2 : class
        {
            string path = null;

            if (!(source is string) && !(source is ValueType))
            {
                if (source is IEnumerable sourceValues)
                {
                    var i = 0;

                    foreach (var value in sourceValues)
                    {
                        var name = $"{sourceName}[{i}]";

                        if (value == null)
                        {
                            continue;
                        }

                        path = value == item ? name : GetPath(value, item, name, value.GetType().GetProperties());

                        if (path != null)
                        {
                            break;
                        }

                        i++;
                    }
                }
                else
                {
                    foreach (var property in sourceProperties)
                    {
                        var value = property.GetValue(source);
                        var name = $"{sourceName}.{property.Name}".Trim('.');

                        if (value == null)
                        {
                            continue;
                        }

                        path = value == item ? name : GetPath(value, item, name, property.PropertyType.GetProperties());

                        if (path != null)
                        {
                            break;
                        }
                    }
                }
            }

            return path;
        }
    }
}