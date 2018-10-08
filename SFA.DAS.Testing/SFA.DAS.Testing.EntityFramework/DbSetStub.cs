using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace SFA.DAS.Testing.EntityFramework
{
    /// <remarks>
    /// See https://docs.microsoft.com/en-us/ef/ef6/fundamentals/testing/writing-test-doubles
    /// </remarks>
    public class DbSetStub<T> : DbSet<T>, IDbAsyncEnumerable<T>, IQueryable<T> where T : class
    {
        public Expression Expression => _query.Expression;
        public Type ElementType => _query.ElementType;
        public override ObservableCollection<T> Local => _local;
        public IQueryProvider Provider => new DbAsyncQueryProviderStub<T>(_query.Provider);

        private readonly IQueryable<T> _query;
        private readonly ObservableCollection<T> _local;

        public DbSetStub(params T[] data) : this(data.AsEnumerable())
        {
        }

        public DbSetStub(IEnumerable<T> data)
        {
            _query = data.AsQueryable();
            _local = new ObservableCollection<T>(_query);
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new DbAsyncEnumeratorStub<T>(_local.GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _local.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region Entity Operations

        public override T Add(T item)
        {
            //todo: clone on the way in?
            _local.Add(item);
            return item;
        }

        public override T Remove(T item)
        {
            _local.Remove(item);
            return item;
        }

        public override T Attach(T item)
        {
            _local.Add(item);
            return item;
        }

        public override T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        #endregion Entity Operations
    }
}