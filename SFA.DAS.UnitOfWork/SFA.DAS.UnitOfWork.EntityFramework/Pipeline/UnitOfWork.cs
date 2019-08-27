using System;
using System.Data.Entity;
using System.Threading.Tasks;
using SFA.DAS.UnitOfWork.Pipeline;

namespace SFA.DAS.UnitOfWork.EntityFramework.Pipeline
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