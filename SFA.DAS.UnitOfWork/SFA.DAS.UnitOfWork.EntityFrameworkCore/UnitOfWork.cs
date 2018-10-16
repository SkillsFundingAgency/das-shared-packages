using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.UnitOfWork.EntityFrameworkCore
{
    public class UnitOfWork<T> : IUnitOfWork where T : DbContext
    {
        private readonly Lazy<T> _db;

        public UnitOfWork(Lazy<T> db)
        {
            _db = db;
        }

        public async Task CommitAsync(Func<Task> next)
        {
            await _db.Value.SaveChangesAsync().ConfigureAwait(false);
            await next().ConfigureAwait(false);
        }
    }
}