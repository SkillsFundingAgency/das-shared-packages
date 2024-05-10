using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Testing.EntityFrameworkCore
{
    public class InMemoryDbAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> innerEnumerator;

        public InMemoryDbAsyncEnumerator(IEnumerator<T> enumerator)
        {
            this.innerEnumerator = enumerator;
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.innerEnumerator.MoveNext());
        }

        public T Current => this.innerEnumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(this.innerEnumerator.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return new ValueTask();
        }
    }
}