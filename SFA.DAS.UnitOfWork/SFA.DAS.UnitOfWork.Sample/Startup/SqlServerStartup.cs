using System;
using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.UnitOfWork.Sample.Startup
{
    public static class SqlServerStartup
    {
        public static IServiceCollection AddSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SampleDb");
            
            EnsureDatabase.For.SqlDatabase(connectionString, 60);
        
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetAssembly(typeof(SqlServerStartup)))
                .WithTransaction()
                .WithExecutionTimeout(TimeSpan.FromMinutes(1))
                .Build()
                .PerformUpgrade();
            
            return services;
        }
    }
}