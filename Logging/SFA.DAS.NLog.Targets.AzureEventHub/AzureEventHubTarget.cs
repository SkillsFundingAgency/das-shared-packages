using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;

namespace SFA.DAS.NLog.Targets.AzureEventHub
{
    [Target("AzureEventHub")]
    public class AzureEventHubTarget : TargetWithLayout
    {
        private EventHubClient _eventHubClient = null;
        private MessagingFactory _messsagingFactory = null;

        [RequiredParameter]
        public string AppName { get; set; }

        /// <summary>
        /// Obsolete - use EventHubConnectionStringKey
        /// </summary>
        [Obsolete("Use EventHubConnectionStringKey")]
        public string EventHubConnectionString { get; set; }

        /// <summary>
        /// Points to the app config setting that contains the EventHubConnectionString. This uses CloudConfiguration.GetSetting which falls back to use Appsetting if not presents
        /// </summary>
        [RequiredParameter]
        public string EventHubConnectionStringKey { get; set; }

        [Obsolete("Use EventHubNameKey")]
        public string EventHubName { get; set; }

        /// <summary>
        /// Points to the app config setting that contains the EventHubName. This uses CloudConfiguration.GetSetting which falls back to use Appsetting if not presents
        /// </summary>
        [RequiredParameter]
        public string EventHubNameKey { get; set; }

        [RequiredParameter]
        public string PartitionKey { get; set; }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(new[] { logEvent });
        }

        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            Send(PartitionKey, logEvents);
        }

        private bool Send(string partitionKey, AsyncLogEventInfo[] logEvents)
        {
            if (_messsagingFactory == null)
            {
                if (!string.IsNullOrWhiteSpace(EventHubConnectionStringKey))
                {
                    _messsagingFactory = MessagingFactory.CreateFromConnectionString(CloudConfigurationManager.GetSetting(EventHubConnectionStringKey));
                }
                else
                {
                    _messsagingFactory = MessagingFactory.CreateFromConnectionString(EventHubConnectionString);
                }
                
            }

            if (_eventHubClient == null)
            {
                var eventHubName = !string.IsNullOrWhiteSpace(CloudConfigurationManager.GetSetting(EventHubNameKey)) 
                                    ? CloudConfigurationManager.GetSetting(EventHubNameKey)
                                    : EventHubName;

                _eventHubClient = _messsagingFactory.CreateEventHubClient(eventHubName);
            }

            var payload = FormPayload(logEvents.Select(e => e.LogEvent), partitionKey);

            _eventHubClient.SendBatch(payload);

            return true;
        }

        private IEnumerable<EventData> FormPayload(IEnumerable<LogEventInfo> logEvents, string partitionKey)
        {
            var payload = new List<EventData>(logEvents?.Count() ?? 0);

            foreach (var logEvent in logEvents)
            {
                var log = new ExpandoObject() as IDictionary<string, object>;

                log.Add("app_Name", AppName);
                log.Add("message", Layout.Render(logEvent));
                log.Add("level", logEvent.Level.Name);
                log.Add("@timestamp", logEvent.TimeStamp);

                foreach (var item in logEvent.Properties)
                {
                    log.Add((string)item.Key, item.Value);
                }
                
                var json = JsonConvert.SerializeObject(log);

                payload.Add(new EventData(Encoding.UTF8.GetBytes(json)) { PartitionKey = partitionKey });
            }

            return payload;
        }
    }
}
