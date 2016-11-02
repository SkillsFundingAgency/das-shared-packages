namespace NLog.Targets
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Azure;

    using Newtonsoft.Json;

    using NLog.Config;

    [Target("Redis")]
    public class RedisTarget : TargetWithLayout
    {
        protected const string ListDataType = "list";
        protected const string ChannelDataType = "channel";

        protected int Db;

        protected string Key;

        public bool IncludeAllProperties { get; set; }

        public string DataType { get; set; }


        [RequiredParameter]
        public string AppName { get; set; }

        [RequiredParameter]
        public string ConnectionStringKey { get; set; }

        [RequiredParameter]
        public string DbSettingsKey { get; set; }

        [RequiredParameter]
        public string KeySettingsKey { get; set; }



        private RedisConnectionManager _redisConnectionManager;

        public RedisTarget()
        {
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            DataType = ListDataType;
            IncludeAllProperties = true;

            int outDb;
            int.TryParse(CloudConfigurationManager.GetSetting(DbSettingsKey), out outDb);
            Db = outDb;

            Key = CloudConfigurationManager.GetSetting(KeySettingsKey);


            var connectionString = CloudConfigurationManager.GetSetting(ConnectionStringKey);

            _redisConnectionManager = new RedisConnectionManager(connectionString, Db);
        }

        protected override void CloseTarget()
        {

            _redisConnectionManager?.Dispose();

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
