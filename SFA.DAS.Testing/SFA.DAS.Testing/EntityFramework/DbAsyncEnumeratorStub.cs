using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Testing.EntityFramework
{
    public class DbAsyncEnumeratorStub<T> : IDbAsyncEnumerator<T>
    {
        public T Current => _inner.Current;
        object IDbAsyncEnumerator.Current => Current;

        private readonly IEnumerator<T> _inner;

        public DbAsyncEnumeratorStub(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }
    }
}