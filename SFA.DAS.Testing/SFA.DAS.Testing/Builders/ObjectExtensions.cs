using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SFA.DAS.Testing.Builders
{
    public static class ObjectExtensions
    {
        public static T Set<T, TProperty>(this T @object, Expression<Func<T, TProperty>> property, TProperty value) where T : class
        {
            var memberExpression = (MemberExpression)property.Body;
            var member = memberExpression.Member;
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = (FieldInfo)member;
                    fieldInfo.SetValue(@object, value);
                    break;
                case MemberTypes.Property:
                    var propertyInfo = (PropertyInfo)member;
                    propertyInfo.SetValue(@object, value);
                    break;
                default:
                    throw new Exception("Set can only be appled to a Field or Property");
            } 
            return @object;
        }

        public static T Add<T, TProperty, TItem>(this T @object, Expression<Func<T, TProperty>> property, TItem item) where T : class where TProperty : IEnumerable<TItem>
        {
            var memberExpression = (MemberExpression)property.Body;
            var collection = GetCollection<T, TItem>(@object, memberExpression);
            collection.Add(item);

            return @object;
        }


        public static T AddRange<T, TProperty, TItem>(this T @object, Expression<Func<T, TProperty>> property, IEnumerable<TItem> items) where T : class where TProperty : IEnumerable<TItem>
        {
            var memberExpression = (MemberExpression)property.Body;
            var collection = GetCollection<T, TItem>(@object, memberExpression);

            foreach (var item in items)
            {
                collection.Add(item);
            }

            return @object;
        }

        private static ICollection<TItem> GetCollection<T, TItem>(T @object, MemberExpression memberExpression)
            where T : class
        {
            ICollection<TItem> collection;

            var member = memberExpression.Member;
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = (FieldInfo)member;
                    collection = (ICollection<TItem>)fieldInfo.GetValue(@object);
                    break;
                case MemberTypes.Property:
                    var propertyInfo = (PropertyInfo)member;
                    collection = (ICollection<TItem>)propertyInfo.GetValue(@object);
                    break;
                default:
                    throw new Exception("Field or Property type expected");
            }

            return collection;
        }
    }
}