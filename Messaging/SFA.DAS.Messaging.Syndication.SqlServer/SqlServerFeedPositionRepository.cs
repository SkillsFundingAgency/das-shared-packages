using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Syndication.SqlServer
{
    public class SqlServerFeedPositionRepository : IFeedPositionRepository
    {
        private readonly string _connectionString;
        private readonly string _getStoredProcedureName;
        private readonly string _updateStoredProcedureName;

        public SqlServerFeedPositionRepository(string connectionString, string getStoredProcedureName, string updateStoredProcedureName)
        {
            _connectionString = connectionString;
            _getStoredProcedureName = getStoredProcedureName;
            _updateStoredProcedureName = updateStoredProcedureName;
        }

        public async Task<string> GetLastSeenMessageIdentifierAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = _getStoredProcedureName;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        return Convert.ToString(await command.ExecuteScalarAsync());
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task UpdateLastSeenMessageIdentifierAsync(string identifier)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = _updateStoredProcedureName;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("Identifier", identifier));

                        await command.ExecuteNonQueryAsync();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
