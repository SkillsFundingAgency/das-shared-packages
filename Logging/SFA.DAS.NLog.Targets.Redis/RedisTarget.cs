namespace NLog.Targets
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using NLog.Config;

    [Target("Redis")]
    public class RedisTarget : TargetWithLayout
    {
        protected const string ListDataType = "list";
        protected const string ChannelDataType = "channel";

        [RequiredParameter]
        public string AppName { get; set; }

        /// <summary>
        /// Sets the host name or IP Address of the redis server
        /// </summary>
        [RequiredParameter]
        public string Host { get; set; }

        /// <summary>
        /// Sets the port number redis is running on
        /// </summary>
        [RequiredParameter]
        public int Port { get; set; }

        /// <summary>
        /// Sets the key to be used for either the list or the pub/sub channel in redis
        /// </summary>
        [RequiredParameter]
        public string Key { get; set; }

        /// <summary>
        /// Sets what redis data type to use, either "list" or "channel"
        /// </summary>
        [RequiredParameter]
        public string DataType { get; set; }

        /// <summary>
        /// Sets the database id to be used in redis if the log entries are sent to a list. Defaults to 0
        /// </summary>
        public int Db { get; set; }

        /// <summary>
        /// Sets the password to be used when accessing Redis with authentication required
        /// </summary>
        public string Password { get; set; }

        public bool IncludeAllProperties { get; set; }

        private RedisConnectionManager _redisConnectionManager;

        public RedisTarget()
        {
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            _redisConnectionManager = new RedisConnectionManager(Host, Port, Db, Password);
        }

        protected override void CloseTarget()
        {
            if (_redisConnectionManager != null)
            {
                _redisConnectionManager.Dispose();
            }

            base.CloseTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var dict = GetDictionary(logEvent, IncludeAllProperties);
            var redisValue = CreateRedisJsonValue(dict);

            var redisDatabase = _redisConnectionManager.GetDatabase();

            switch (DataType.ToLower())
            {
                case ListDataType:
                    redisDatabase.ListRightPush(Key, redisValue);
                    break;
                case ChannelDataType:
                    redisDatabase.Publish(Key, redisValue);
                    break;
                default:
                    throw new Exception("no data type defined for redis");
            }
        }

        protected override void Write(Common.AsyncLogEventInfo logEvent)
        {
            var dict = GetDictionary(logEvent.LogEvent, IncludeAllProperties);
            var redisValue = CreateRedisJsonValue(dict);

            var redisDatabase = _redisConnectionManager.GetDatabase();
            switch (DataType.ToLower())
            {
                case ListDataType:
                    redisDatabase.ListRightPushAsync(Key, redisValue);
                    break;
                case ChannelDataType:
                    redisDatabase.PublishAsync(Key, redisValue);
                    break;
                default:
                    throw new Exception("no data type defined for redis");
            }
        }

        private IDictionary<object, object> GetDictionary(LogEventInfo logEvent, bool includeProperties)
        {
            var message = Layout.Render(logEvent);
            var properties = !includeProperties 
                ? new Dictionary<object, object>() 
                : logEvent.Properties;
            
            properties.Add("message", message);
            properties.Add("level", logEvent.Level.Name);
            properties.Add("app_Name", AppName);
            properties.Add("@timestamp", logEvent.TimeStamp);

            return properties;
        }

        private string CreateRedisJsonValue(IDictionary<object, object> properties)
        {
            return JsonConvert.SerializeObject(properties);
        }
    }
}
