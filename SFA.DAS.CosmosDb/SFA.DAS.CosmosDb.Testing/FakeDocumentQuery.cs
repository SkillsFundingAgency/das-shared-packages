using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace SFA.DAS.CosmosDb.Testing
{
    public class FakeDocumentQuery<T> : IDocumentQuery<T>, IOrderedQueryable<T>
    {
        public Expression Expression => _query.Expression;
        public Type ElementType => _query.ElementType;
        public bool HasMoreResults => ++_page <= _pages;
        public IQueryProvider Provider => new FakeDocumentQueryProvider<T>(_query.Provider);

        private readonly IQueryable<T> _query;
        private readonly int _pages = 1;
        private int _page = 0;

        public FakeDocumentQuery(IEnumerable<T> data)
        {
            _query = data.AsQueryable();
        }

        public Task<FeedResponse<TResult>> ExecuteNextAsync<TResult>(CancellationToken token = default)
        {
            return Task.FromResult(new FeedResponse<TResult>(this.Cast<TResult>()));
        }

        public Task<FeedResponse<dynamic>> ExecuteNextAsync(CancellationToken token = default)
        {
            return Task.FromResult(new FeedResponse<dynamic>(this.Cast<dynamic>()));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _query.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
        }
    }
}