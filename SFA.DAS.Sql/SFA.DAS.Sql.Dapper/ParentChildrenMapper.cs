using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SFA.DAS.Sql.Dapper
{
    public sealed class ParentChildrenMapper<TParent, TChild> where TParent : class
    {
        private static readonly ConcurrentDictionary<Expression<Func<TParent, IList<TChild>>>,
            Func<TParent, IList<TChild>>> ParentChildLookup =
            new ConcurrentDictionary<Expression<Func<TParent, IList<TChild>>>,
                Func<TParent, IList<TChild>>>();

        private static Func<TParent, IList<TChild>> CachedCompiledChildFunc(
            Expression<Func<TParent, IList<TChild>>> expression)
        {
            Func<TParent, IList<TChild>> func;
            if (ParentChildLookup.ContainsKey(expression))
            {
                if (ParentChildLookup.TryGetValue(expression, out func))
                {
                    return func;
                }
            }
            func = expression.Compile();
            ParentChildLookup.TryAdd(expression, func);
            return func;
        }

        public Func<TParent, TChild, TParent> Map<T>(
            Dictionary<T, TParent> lookup,
            Func<TParent, T> parentIdentifierProperty,
            Expression<Func<TParent, IList<TChild>>> parentChildrenProperty)
        {
            if (lookup == null)
            {
                throw new ArgumentNullException(nameof(lookup));
            }

            return (x, y) =>
            {
                TParent parent;
                if (!lookup.TryGetValue(parentIdentifierProperty.Invoke(x), out parent))
                {
                    lookup.Add(parentIdentifierProperty.Invoke(x), parent = x);
                }

                if (y != null)
                {
                    var func = CachedCompiledChildFunc(parentChildrenProperty);
                    var children = func.Invoke(parent);
                    if (children == null)
                    {
                        children = new List<TChild>();
                        var property = (PropertyInfo)((MemberExpression)parentChildrenProperty.Body).Member;
                        property.SetValue(parent, children, null);
                    }
                    children.Add(y);
                }

                return parent;
            };
        }
    }
}
