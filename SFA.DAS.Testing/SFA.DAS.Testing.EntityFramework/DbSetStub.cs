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
    public class DbSetStub<T> : DbSet<T>, IDbAsyncEnumerable<T>, IQueryable<T> where T : class
    {
        public Expression Expression => _data.Expression;
        public Type ElementType => _data.ElementType;
        public override ObservableCollection<T> Local => _local ?? (_local = new ObservableCollection<T>(_data));
        public IQueryProvider Provider => new DbAsyncQueryProviderStub<T>(_data.Provider);

        private readonly IQueryable<T> _data;
        private ObservableCollection<T> _local;

        public DbSetStub(params T[] data) : this(data.AsEnumerable())
        {
        }

        public DbSetStub(IEnumerable<T> data)
        {
            _data = data.AsQueryable();
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new DbAsyncEnumeratorStub<T>(_data.GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}