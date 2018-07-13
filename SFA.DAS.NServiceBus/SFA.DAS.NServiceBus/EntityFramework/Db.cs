using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public class Db<T> : IDb where T : DbContext
    {
        private readonly Lazy<T> _db;

        public Db(Lazy<T> db)
        {
            _db = db;
        }

        public Task SaveChangesAsync()
        {
            return _db.Value.SaveChangesAsync();
        }
    }
}