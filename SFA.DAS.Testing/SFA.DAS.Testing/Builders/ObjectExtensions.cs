using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SFA.DAS.Testing.Builders
{
    public static class ObjectExtensions
    {
        internal static T Set<T, TProperty>(this T @object, Expression<Func<T, TProperty>> property, TProperty value) where T : class
        {
            var memberExpression = (MemberExpression)property.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;

            propertyInfo.SetValue(@object, value);

            return @object;
        }

        internal static T Add<T, TProperty, TItem>(this T @object, Expression<Func<T, TProperty>> property, TItem item) where T : class where TProperty : IEnumerable<TItem>
        {
            var memberExpression = (MemberExpression)property.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var collection = (ICollection<TItem>)propertyInfo.GetValue(@object);

            collection.Add(item);

            return @object;
        }

        internal static T AddRange<T, TProperty, TItem>(this T @object, Expression<Func<T, TProperty>> property, IEnumerable<TItem> items) where T : class where TProperty : IEnumerable<TItem>
        {
            var memberExpression = (MemberExpression)property.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var collection = (ICollection<TItem>)propertyInfo.GetValue(@object);

            foreach (var item in items)
            {
                collection.Add(item);
            }

            return @object;
        }
    }
}