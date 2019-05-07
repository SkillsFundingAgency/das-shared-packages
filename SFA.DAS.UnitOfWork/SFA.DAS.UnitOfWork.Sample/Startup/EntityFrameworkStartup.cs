using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Persistence;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.UnitOfWork.Sample.Data;

namespace SFA.DAS.UnitOfWork.Sample.Startup
{
    public static class EntityFrameworkStartup
    {
        public static IServiceCollection AddEntityFramework(this IServiceCollection services)
        {
            return services.AddScoped(p =>
            {
                var unitOfWorkContext = p.GetService<IUnitOfWorkContext>();
                var synchronizedStorageSession = unitOfWorkContext.Get<SynchronizedStorageSession>();
                var sqlStorageSession = synchronizedStorageSession.GetSqlStorageSession();
                var optionsBuilder = new DbContextOptionsBuilder<SampleDbContext>().UseSqlServer(sqlStorageSession.Connection);
                var dbContext = new SampleDbContext(optionsBuilder.Options);
            
                dbContext.Database.UseTransaction(sqlStorageSession.Transaction);

                return dbContext;
            });
        }
    }
}