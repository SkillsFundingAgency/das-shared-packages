using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SFA.DAS.Testing.Builders
{
    public static class ObjectExtensions
    {
        public static TObject Set<TObject, TProperty>(this TObject @object, Expression<Func<TObject, TProperty>> property, TProperty value) where TObject : class
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
                    throw new InvalidOperationException("Set can only be appled to a Field or Property");
            }
            return @object;
        }

        public static TObject Add<TObject, TProperty, TItem>(this TObject @object, Expression<Func<TObject, TProperty>> property, TItem item) where TObject : class where TProperty : IEnumerable<TItem>
        {
            var memberExpression = (MemberExpression)property.Body;
            var collection = GetCollection<TObject, TItem>(@object, memberExpression);
            collection.Add(item);

            return @object;
        }


        public static TObject AddRange<TObject, TProperty, TItem>(this TObject @object, Expression<Func<TObject, TProperty>> property, IEnumerable<TItem> items) where TObject : class where TProperty : IEnumerable<TItem>
        {
            var memberExpression = (MemberExpression)property.Body;
            var collection = GetCollection<TObject, TItem>(@object, memberExpression);

            foreach (var item in items)
            {
                collection.Add(item);
            }

            return @object;
        }

        private static ICollection<TItem> GetCollection<TObject, TItem>(TObject @object, MemberExpression memberExpression)
            where TObject : class
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
                    throw new InvalidOperationException("Field or Property type expected");
            }

            return collection;
        }
    }
}