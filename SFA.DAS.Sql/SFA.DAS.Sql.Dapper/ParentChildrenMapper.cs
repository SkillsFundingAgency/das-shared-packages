using System;
using System.Collections.Generic;

namespace SFA.DAS.Sql.Dapper
{
    public sealed class ParentChildrenMapper<TParent, TChild>
    {
        public Func<TParent, TChild, TParent> Map<T>(
            Dictionary<T, TParent> lookup,
            Func<TParent, T> parentIdentifierProperty,
            Func<TParent, IList<TChild>> parentChildrenProperty)
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
                    var children = parentChildrenProperty.Invoke(parent) ?? new List<TChild>();
                    children.Add(y);
                }

                return parent;
            };
        }
    }
}