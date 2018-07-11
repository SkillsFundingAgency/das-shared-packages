using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace SFA.DAS.Testing.EntityFramework
{
    public class DbAsyncEnumerableStub<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        IQueryProvider IQueryable.Provider => new DbAsyncQueryProviderStub<T>(this);

        public DbAsyncEnumerableStub(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        public DbAsyncEnumerableStub(Expression expression)
            : base(expression)
        {
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new DbAsyncEnumeratorStub<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }
    }
}