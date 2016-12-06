using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using NLog;

namespace SFA.DAS.NLog.Logger
{
    public sealed class NLogLogger : ILog
    {
        private readonly IRequestContext _context;
        private readonly string _loggerType;
        private readonly string _version;

        public NLogLogger(Type loggerType, IRequestContext context)
        {
            _loggerType = loggerType?.ToString() ?? "DefaultWebLogger";
            _context = context;
            _version = GetVersion();
        }

        public void Trace(string message)
        {
            SendLog(message, LogLevel.Trace);
        }

        public void Trace(string message, ILogEntry logEntry)
        {
            SendLog(message, LogLevel.Trace, new Dictionary<string, object> { { GetLogEntryName(logEntry), logEntry } });
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
            SendLog(message, LogLevel.Debug, new Dictionary<string, object> { { GetLogEntryName(logEntry), logEntry } });
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
            SendLog(message, LogLevel.Info, new Dictionary<string, object> { { GetLogEntryName(logEntry), logEntry } });
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
            SendLog(message, LogLevel.Warn, new Dictionary<string, object> { { GetLogEntryName(logEntry), logEntry } });
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
            SendLog(message, LogLevel.Warn, new Dictionary<string, object> { { GetLogEntryName(logEntry), logEntry } }, ex);
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
            SendLog(message, LogLevel.Error, new Dictionary<string, object> { { GetLogEntryName(logEntry), logEntry } }, ex);
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
            SendLog(message, LogLevel.Fatal, new Dictionary<string, object> { { GetLogEntryName(logEntry), logEntry } }, ex);
        }

        public void Fatal(Exception ex, string message, IDictionary<string, object> properties)
        {
            SendLog(message, LogLevel.Fatal, properties, ex);
        }

        private string GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
        private static string GetLogEntryName(ILogEntry logEntry)
        {
            return logEntry.GetType().Name.Replace("LogEntry", string.Empty);
        }

        private void SendLog(object message, LogLevel level, Exception exception = null)
        {
            SendLog(message, level, new Dictionary<string, object>(), exception);
        }

        private void SendLog(object message, LogLevel level, IDictionary<string, object> properties, Exception exception = null)
        {
            IDictionary<string, object> propertiesLocal = null;

            propertiesLocal = (properties == null) ? new Dictionary<string, object>() : properties;

            propertiesLocal.Add("LoggerType", _loggerType);
            propertiesLocal.Add("RequestCtx", _context);
            propertiesLocal.Add("Version", _version);

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
