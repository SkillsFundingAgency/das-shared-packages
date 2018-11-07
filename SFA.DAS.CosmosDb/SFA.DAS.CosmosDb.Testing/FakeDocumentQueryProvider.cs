using System.Linq;
using System.Linq.Expressions;

namespace SFA.DAS.CosmosDb.Testing
{
    public class FakeDocumentQueryProvider<T> : IQueryProvider
    {
        private readonly IQueryProvider _queryProvider;

        public FakeDocumentQueryProvider(IQueryProvider queryProvider)
        {
            _queryProvider = queryProvider;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new FakeDocumentQuery<T>(_queryProvider.CreateQuery<T>(expression));
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new FakeDocumentQuery<TElement>(_queryProvider.CreateQuery<TElement>(expression));
        }

        public object Execute(Expression expression)
        {
            return _queryProvider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _queryProvider.Execute<TResult>(expression);
        }
    }
}