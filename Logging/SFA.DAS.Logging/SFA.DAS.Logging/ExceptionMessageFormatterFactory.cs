using System;
using System.Collections.Concurrent;
using System.Linq;
using SFA.DAS.Logging.MessageFormatters;
using ExceptionMessageFormatter = SFA.DAS.Logging.MessageFormatters.ExceptionMessageFormatter;

namespace SFA.DAS.Logging
{
    internal static class ExceptionMessageFormatterFactory
    {
        private static readonly ConcurrentDictionary<Type, IExceptionMessageFormatter> ExceptionMessageFormatterCache =
            new ConcurrentDictionary<Type, IExceptionMessageFormatter>();

        private static IExceptionMessageFormatter GeneralExceptionFormatter => new ExceptionMessageFormatter();

        private static readonly IExceptionMessageFormatter[] ExceptionFormatters =
        {
            new AggregateExceptionMessageFormatter(),
            new HttpRequestExceptionMessageFormatter(),
            new ExceptionMessageFormatter()
        };

        public static IExceptionMessageFormatter GetFormatter(Exception exception)
        {
            return ExceptionMessageFormatterCache.GetOrAdd(exception.GetType(), GetExceptionFormatter(exception));
        }

        private static IExceptionMessageFormatter GetExceptionFormatter(Exception exception)
        {
            IExceptionMessageFormatter formatter = null;

            if (exception == null)
            {
                return GeneralExceptionFormatter;
            }

            var exceptionType = exception.GetType();

            while (formatter == null)
            {
                // convert to dictionary
                formatter = ExceptionFormatters.FirstOrDefault(ef => ef.SupportedException == exceptionType);

                if (formatter != null) continue;

                if (exceptionType.BaseType == null)
                {
                    return GeneralExceptionFormatter;
                }

                exceptionType = exceptionType.BaseType;
            }

            return formatter;
        }
    }
}
