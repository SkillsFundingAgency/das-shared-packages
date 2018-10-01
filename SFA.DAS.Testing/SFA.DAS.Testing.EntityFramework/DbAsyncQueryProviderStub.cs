using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Testing.EntityFramework
{
    public class DbAsyncQueryProviderStub<TEntity> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public DbAsyncQueryProviderStub(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            if (expression is MethodCallExpression methodCallExpression)
            {
                var resultType = methodCallExpression.Method.ReturnType;
                var elementType = resultType.GetGenericArguments()[0];
                var queryableType = typeof(DbAsyncEnumerableStub<>).MakeGenericType(elementType);

                return (IQueryable)Activator.CreateInstance(queryableType, expression);
            }

            return new DbAsyncEnumerableStub<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DbAsyncEnumerableStub<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }
}