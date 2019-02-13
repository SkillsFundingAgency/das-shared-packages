using Newtonsoft.Json;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using System.Collections.Generic;

namespace SFA.DAS.NLog.Targets.Redis.DotNetCore
{
    [Target("Redis")]
    public sealed class RedisTarget : TargetWithLayout
    {
        private const string AppNameKey = "app_Name";
        private RedisConnectionManager _redisConnectionManager;
        private string _key = "logstash";
        private string _connectionString;
        private string _environment;

        [RequiredParameter]
        public string AppName { get; set; }

        [RequiredParameter]
        public string ConnectionStringName { get; set; }

        public string EnvironmentKeyName { get; set; }

        public bool IncludeAllProperties { get; set; }

        [ArrayParameter(typeof(Field), "field")]
        public IList<Field> Fields { get; set; } = new List<Field>();

        public RedisTarget()
        {
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            _environment = EnvironmentKeyName?.GetConfigurationValue();
            _connectionString = ConnectionStringName.GetConnectionString();
            _redisConnectionManager = new RedisConnectionManager(_connectionString);
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

            redisDatabase.ListRightPush(_key, redisValue);
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            var dict = GetDictionary(logEvent.LogEvent, IncludeAllProperties);
            var redisValue = CreateRedisJsonValue(dict);

            var redisDatabase = _redisConnectionManager.GetDatabase();

            redisDatabase.ListRightPushAsync(_key, redisValue);
        }

        private IDictionary<object, object> GetDictionary(LogEventInfo logEvent, bool includeProperties)
        {
            var message = Layout.Render(logEvent);
            var properties = !includeProperties 
                ? new Dictionary<object, object>() 
                : logEvent.Properties;
            
            properties.Add("message", message);
            properties.Add("level", logEvent.Level.Name);
            properties.Add("@timestamp", logEvent.TimeStamp);
            if (!properties.ContainsKey(AppNameKey))
            {
                properties.Add(AppNameKey, AppName);
            }

            if (!properties.ContainsKey("Environment"))
            {
                var environment = string.IsNullOrEmpty(_environment) ? "Dev" : _environment;

                properties.Add("Environment", environment);
            }

            foreach (var field in Fields)
            {
                var renderedField = field.Layout.Render(logEvent);
                if (!string.IsNullOrWhiteSpace(renderedField))
                    properties.Add(field.Name, renderedField.ToSystemType(field.LayoutType, logEvent.FormatProvider));
            }

            if (logEvent.Exception != null)
            {
                if (!properties.ContainsKey("Exception"))
                {
                    dynamic innerException = null;

                    if (logEvent.Exception.InnerException != null)
                    {
                        innerException = new { message = logEvent.Exception.InnerException.Message, source = logEvent.Exception.InnerException.Source, stackTrace = logEvent.Exception.InnerException.StackTrace, type = logEvent.Exception.InnerException.GetType().Name };
                    }

                    properties.Add("Exception", new { message = logEvent.Exception.GetMessage(), source = logEvent.Exception.Source, innerException = innerException, stackTrace = logEvent.Exception.StackTrace, type = logEvent.Exception.GetType().Name });
                }
            }

            return properties;
        }

        private string CreateRedisJsonValue(IDictionary<object, object> properties)
        {
            return JsonConvert.SerializeObject(properties);
        }

    }
}
