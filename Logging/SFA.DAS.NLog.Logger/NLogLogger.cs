using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using NLog;

namespace SFA.DAS.NLog.Logger
{
    public class NLogLogger : ILog
    {
        private readonly IRequestContext _context;
        private readonly IDictionary<string, object> _properties;
        private readonly string _version;
        private readonly string _loggerType;

        public NLogLogger(Type loggerType = null, IRequestContext context = null, IDictionary<string, object> properties = null)
        {
            _loggerType = loggerType?.ToString() ?? "DefaultLogger";
            _context = context;
            _properties = properties;
            _version = GetVersion(loggerType);
        }

        public string ApplicationName { get; set; }

        public string GetMappedDiagnosticsLogicalContext(string name)
        {
            return MappedDiagnosticsLogicalContext.Get(name);
        }

        public void Trace(string message)
        {
            SendLog(message, LogLevel.Trace);
        }

        public void Trace(string message, ILogEntry logEntry)
        {
            SendLog(message, LogLevel.Trace, BuildProperties(logEntry));
        }

        public void Trace(string message, IDictionary<string, object> properties)
        {
            SendLog(message, LogLevel.Trace, properties);
        }

        public void Debug(string message)
        {
            SendLog(message, LogLevel.Debug);
        }

        public void Debug(string message, ILogEntry logEntry)
        {
            SendLog(message, LogLevel.Debug, BuildProperties(logEntry));
        }

        public void Debug(string message, IDictionary<string, object> properties)
        {
            SendLog(message, LogLevel.Debug, properties);
        }

        public void Info(string message)
        {
            SendLog(message, LogLevel.Info);
        }

        public void Info(string message, ILogEntry logEntry)
        {
            SendLog(message, LogLevel.Info, BuildProperties(logEntry));
        }

        public void Info(string message, IDictionary<string, object> properties)
        {
            SendLog(message, LogLevel.Info, properties);
        }

        public void Warn(string message)
        {
            SendLog(message, LogLevel.Warn);
        }

        public void Warn(string message, ILogEntry logEntry)
        {
            SendLog(message, LogLevel.Warn, BuildProperties(logEntry));
        }

        public void Warn(string message, IDictionary<string, object> properties)
        {
            SendLog(message, LogLevel.Warn, properties);
        }

        public void Warn(Exception ex, string message)
        {
            SendLog(message, LogLevel.Warn, ex);
        }

        public void Warn(Exception ex, string message, ILogEntry logEntry)
        {
            SendLog(message, LogLevel.Warn, BuildProperties(logEntry));
        }

        public void Warn(Exception ex, string message, IDictionary<string, object> properties)
        {
            SendLog(message, LogLevel.Warn, properties, ex);
        }

        public void Error(Exception ex, string message)
        {
            SendLog(message, LogLevel.Error, ex);
        }

        public void Error(Exception ex, string message, ILogEntry logEntry)
        {
            SendLog(message, LogLevel.Error, BuildProperties(logEntry), ex);
        }

        public void Error(Exception ex, string message, IDictionary<string, object> properties)
        {
            SendLog(message, LogLevel.Error, properties, ex);
        }

        public void Fatal(Exception ex, string message)
        {
            SendLog(message, LogLevel.Fatal, ex);
        }

        public void Fatal(Exception ex, string message, ILogEntry logEntry)
        {
            SendLog(message, LogLevel.Fatal, BuildProperties(logEntry), ex);
        }

        public void Fatal(Exception ex, string message, IDictionary<string, object> properties)
        {
            SendLog(message, LogLevel.Fatal, properties, ex);
        }

        private string GetVersion(Type callingType)
        {
            if (callingType == null) return null;

            var assembly = callingType.Assembly;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }

        private void SendLog(object message, LogLevel level, Exception exception = null)
        {
            SendLog(message, level, new Dictionary<string, object>(), exception);
        }

        private IDictionary<string, object> BuildProperties(ILogEntry entry)
        {
            var properties = BuilPropertiesFromEntry(entry);
            AddPropertiesFromContext(properties);
            return properties;
        }

        private static Dictionary<string, object> BuilPropertiesFromEntry(ILogEntry entry)
        {
            if (entry == null)
                return new Dictionary<string, object>();

            var entryProperties = entry.GetType().GetProperties();
            var properties = new Dictionary<string, object>(entryProperties.Length);

            foreach (var property in entryProperties)
            {
                var name = property.Name;
                var value = property.GetValue(entry);

                properties.Add(name, value);
            }
            return properties;
        }

        private void AddPropertiesFromContext(Dictionary<string, object> properties)
        {
            var correlationId = MappedDiagnosticsLogicalContext.Get("CorrelationId");
            if (!string.IsNullOrEmpty(correlationId)) properties.Add("CorrelationId", correlationId);
        }

        private void SendLog(object message, LogLevel level, IDictionary<string, object> properties, Exception exception = null)
        {
            var propertiesLocal = properties ?? new Dictionary<string, object>();

            foreach (var property in _properties ?? new Dictionary<string, object>())
            {
                if (!propertiesLocal.ContainsKey(property.Key))
                {
                    propertiesLocal.Add(property);
                }
            }

            if (_context != null)
                propertiesLocal.Add("RequestCtx", _context);

            if (!propertiesLocal.ContainsKey("LoggerType"))
            {
                propertiesLocal.Add("LoggerType", _loggerType);
            }

            if (!propertiesLocal.ContainsKey("Version"))
            {
                propertiesLocal.Add("Version", _version);
            }

            if (!propertiesLocal.ContainsKey("LogTimestamp"))
            {
                propertiesLocal.Add("LogTimestamp", DateTime.UtcNow.ToString("o"));
            }

            if (!propertiesLocal.ContainsKey("app_Name") && !string.IsNullOrEmpty(ApplicationName))
            {
                propertiesLocal.Add("app_Name", ApplicationName);
            }

            var logEvent = new LogEventInfo(level, _loggerType, message.ToString());
            logEvent.Exception = exception;

            foreach (var property in propertiesLocal)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            ILogger log = LogManager.GetCurrentClassLogger();
            log.Log(logEvent);
        }
    }
}
