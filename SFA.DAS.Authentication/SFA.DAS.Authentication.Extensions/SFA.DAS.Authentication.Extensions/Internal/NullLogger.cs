using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Authentication.Extensions
{
    internal class NullLogger : ILog
    {
        public void Debug(string message)
        {
            return;
        }

        public void Debug(string message, IDictionary<string, object> properties)
        {
            return;
        }

        public void Debug(string message, ILogEntry logEntry)
        {
            return;
        }

        public void Error(Exception ex, string message)
        {
            return;
        }

        public void Error(Exception ex, string message, IDictionary<string, object> properties)
        {
            return;
        }

        public void Error(Exception ex, string message, ILogEntry logEntry)
        {
            return;
        }

        public void Fatal(Exception ex, string message)
        {
            return;
        }

        public void Fatal(Exception ex, string message, IDictionary<string, object> properties)
        {
            return;
        }

        public void Fatal(Exception ex, string message, ILogEntry logEntry)
        {
            return;
        }

        public void Info(string message)
        {
            return;
        }

        public void Info(string message, IDictionary<string, object> properties)
        {
            return;
        }

        public void Info(string message, ILogEntry logEntry)
        {
            return;
        }

        public void Trace(string message)
        {
            return;
        }

        public void Trace(string message, IDictionary<string, object> properties)
        {
            return;
        }

        public void Trace(string message, ILogEntry logEntry)
        {
            return;
        }

        public void Warn(string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(string message, IDictionary<string, object> properties)
        {
            return;
        }

        public void Warn(string message, ILogEntry logEntry)
        {
            return;
        }

        public void Warn(Exception ex, string message)
        {
            return;
        }

        public void Warn(Exception ex, string message, IDictionary<string, object> properties)
        {
            return;
        }

        public void Warn(Exception ex, string message, ILogEntry logEntry)
        {
            return;
        }
    }

}
