using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SFA.DAS.Validation.Extensions
{
    public static class ObjectExtensions
    {
        public static string GetPath(this object source, object item, string sourceName = "")
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return source.Equals(item) ? sourceName : GetPath(source, item, sourceName, source.GetType().GetProperties());
        }

        private static string GetPath(object source, object item, string sourceName, IEnumerable<PropertyInfo> sourceProperties)
        {
            string path = null;

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

                    path = value.Equals(item) ? name : GetPath(value, item, name, value.GetType().GetProperties());

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

                    path = value.Equals(item) ? name : GetPath(value, item, name, property.PropertyType.GetProperties());

                    if (path != null)
                    {
                        break;
                    }
                }
            }

            return path;
        }
    }
}