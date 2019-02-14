using System;
using System.Collections.Concurrent;
using System.Linq;
using SFA.DAS.ExceptionHandling.MessageFormatters;

namespace SFA.DAS.ExceptionHandling
{
    public class ExceptionMessageFormatterFactory : IExceptionMessageFormatterFactory
    {
        private readonly ConcurrentDictionary<Type, IExceptionMessageFormatter> _exceptionMessageFormatterCache =
            new ConcurrentDictionary<Type, IExceptionMessageFormatter>();

        private IExceptionMessageFormatter GeneralExceptionFormatter => new ExceptionMessageFormatter();

        private readonly IExceptionMessageFormatter[] _exceptionFormatters;

        public ExceptionMessageFormatterFactory(IExceptionMessageFormatter[] formatters)
        {
            _exceptionFormatters = formatters;
        }

        public IExceptionMessageFormatter GetFormatter(Exception exception)
        {
            return _exceptionMessageFormatterCache.GetOrAdd(exception.GetType(), GetExceptionFormatter(exception));
        }

        private IExceptionMessageFormatter GetExceptionFormatter(Exception exception)
        {
            if (exception == null)
            {
                return GeneralExceptionFormatter;
            }

            var exceptionType = exception.GetType();

            var formatter = GetBestMatchFormatterForExceptionType(exceptionType);

            return formatter;
        }

        private IExceptionMessageFormatter GetBestMatchFormatterForExceptionType(Type exceptionType)
        {
            while (true)
            {
                var formatter = _exceptionFormatters.FirstOrDefault(ef => ef.SupportedException == exceptionType);

                // we found a formatter that can handle this specific exception type
                if (formatter != null)
                {
                    return formatter;
                }

                // No handler for this specific exception type - now try the parent class
                if (exceptionType.BaseType == null)
                {
                    return GeneralExceptionFormatter;
                }

                exceptionType = exceptionType.BaseType;
            }
        }
    }
}
