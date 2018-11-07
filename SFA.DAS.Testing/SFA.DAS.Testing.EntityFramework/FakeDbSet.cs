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
    public class FakeDbSet<T> : DbSet<T>, IDbAsyncEnumerable<T>, IQueryable<T> where T : class
    {
        public Expression Expression => _query.Expression;
        public Type ElementType => _query.ElementType;
        public override ObservableCollection<T> Local => _local;
        public IQueryProvider Provider => new FakeDbAsyncQueryProvider<T>(_query.Provider);

        private readonly IQueryable<T> _query;
        private readonly ObservableCollection<T> _local;

        public FakeDbSet(params T[] data) : this(data.AsEnumerable())
        {
        }

        public FakeDbSet(IEnumerable<T> data)
        {
            _query = data.AsQueryable();
            _local = new ObservableCollection<T>(_query);
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new FakeDbAsyncEnumerator<T>(_local.GetEnumerator());
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

        public override T Add(T item)
        {
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
    }
}