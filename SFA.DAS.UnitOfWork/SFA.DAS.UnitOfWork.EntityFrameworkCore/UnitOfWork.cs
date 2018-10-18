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
            using (new SynchronizationContextRemover())
            {
                await _db.Value.SaveChangesAsync();
            }
            
            await next().ConfigureAwait(false);
        }
    }
}