namespace NLog.Targets
{
    using System;

    using StackExchange.Redis;

    internal class RedisConnectionManager : IDisposable
    {
        private ConnectionMultiplexer _connectionMultiplexer;

        public RedisConnectionManager(string connectionString)
        {
            InitializeConnection(connectionString);
        }

        private void InitializeConnection(string connectionString)
        {
            var connectionOptions = ConfigurationOptions.Parse(connectionString);
            _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionOptions);
        }

        public IDatabase GetDatabase()
        {
            if (_connectionMultiplexer == null) throw new Exception("connection manager not initialized");

            return _connectionMultiplexer.GetDatabase();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connectionMultiplexer != null)
                {
                    _connectionMultiplexer.Dispose();
                }
            }
        }
    }
}
