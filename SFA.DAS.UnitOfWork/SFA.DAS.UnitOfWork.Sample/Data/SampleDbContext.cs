using Microsoft.EntityFrameworkCore;
using SFA.DAS.UnitOfWork.Sample.Models;

namespace SFA.DAS.UnitOfWork.Sample.Data
{
    public class SampleDbContext : DbContext
    {
        public DbSet<Foobar> Foobars { get; set; }
        
        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
        {
        }
    }
}