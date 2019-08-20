using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.SqlServer.ClientOutbox
{
    public static class ConnectionBuilderExtensions
    {
        public static async Task<DbConnection> OpenConnectionAsync(this Func<DbConnection> connectionBuilder)
        {
            var connection = connectionBuilder();
            
            try
            {
                await connection.OpenAsync().ConfigureAwait(false);
                
                return connection;
            }
            catch
            {
                connection.Dispose();
                
                throw;
            }
        }
    }
}