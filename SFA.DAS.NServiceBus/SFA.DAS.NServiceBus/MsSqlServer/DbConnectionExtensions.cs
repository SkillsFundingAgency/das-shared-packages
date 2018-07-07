using System.Data.Common;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.MsSqlServer
{
    public static class DbConnectionExtensions
    {
        public static async Task<DbConnection> TryOpenAsync(this DbConnection connection)
        {
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