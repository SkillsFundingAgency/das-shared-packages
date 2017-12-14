using System;
using System.Collections.Generic;
using SFA.DAS.NLog.Logger;

namespace PublishReceiveSample
{
    class SampleLogger : ILog
    {
        public void Trace(string message)
        {
            Console.WriteLine(message);
        }

        public void Trace(string message, IDictionary<string, object> properties)
        {
            Console.WriteLine(message);
        }

        public void Trace(string message, ILogEntry logEntry)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message, IDictionary<string, object> properties)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message, ILogEntry logEntry)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message, IDictionary<string, object> properties)
        {
            Console.WriteLine(message);
        }

        public void Info(string message, ILogEntry logEntry)
        {
            Console.WriteLine(message);
        }

        public void Warn(string message)
        {
            Console.WriteLine(message);
        }

        public void Warn(string message, IDictionary<string, object> properties)
        {
            Console.WriteLine(message);
        }

        public void Warn(string message, ILogEntry logEntry)
        {
            Console.WriteLine(message);
        }

        public void Warn(Exception ex, string message)
        {
             Console.WriteLine(message);
        }

        public void Warn(Exception ex, string message, IDictionary<string, object> properties)
        {
             Console.WriteLine(message);
        }

        public void Warn(Exception ex, string message, ILogEntry logEntry)
        {
             Console.WriteLine(message);
        }

        public void Error(Exception ex, string message)
        {
             Console.WriteLine(message);
        }

        public void Error(Exception ex, string message, IDictionary<string, object> properties)
        {
             Console.WriteLine(message);
        }

        public void Error(Exception ex, string message, ILogEntry logEntry)
        {
             Console.WriteLine(message);
        }

        public void Fatal(Exception ex, string message)
        {
             Console.WriteLine(message);
        }

        public void Fatal(Exception ex, string message, IDictionary<string, object> properties)
        {
             Console.WriteLine(message);
        }

        public void Fatal(Exception ex, string message, ILogEntry logEntry)
        {
             Console.WriteLine(message);
        }
    }
}
