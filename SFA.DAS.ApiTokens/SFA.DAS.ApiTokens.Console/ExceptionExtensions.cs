using System;
using System.Reflection;
using System.Text;

namespace SFA.DAS.ApiTokens.Console
{
    /// <summary>
    /// Extension methods for exception classes.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Format exception information. Can be used to prepare an
        /// exception to be written to the event log
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="includeDetails">True to include stack trace details in the output</param>
        public static string FormatException(this Exception exception, bool includeDetails = false)
        {
            var sb = new StringBuilder();

            var currentException = exception;

            while (currentException != null)
            {
                sb.Append("==> " + currentException.Message);

                // reflection info
                if (currentException is ReflectionTypeLoadException)
                {
                    foreach (var reflectionLoadException in (currentException as ReflectionTypeLoadException).LoaderExceptions)
                        sb.AppendFormat("... {0}\n", reflectionLoadException.Message);
                }

                if (includeDetails)
                {
                    sb.AppendLine(currentException.GetType().ToString());
                    sb.Append(currentException.StackTrace);

                    // include any exception data
                    if (currentException.Data.Count > 0)
                    {
                        sb.AppendLine("Exception data:");
                        foreach (object key in currentException.Data.Keys)
                            sb.AppendFormat("... '{0}' = {1}\n", key, currentException.Data[key]);
                    }
                }

                currentException = currentException.InnerException;
            }

            return sb.ToString();
        }
    }
}
