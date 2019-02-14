using System;
using System.Text;
using SFA.DAS.ExceptionHandling.MessageFormatters;

namespace SFA.DAS.ExceptionHandling
{
    public static class ExceptionExtensions
    {
        /// <summary>
        ///     Set up a default which can handle all exceptions.
        ///     During app start up a new factory should be registered using <see cref="UseExceptionMessageFormatterFactory"/>
        /// </summary>
        private static IExceptionMessageFormatterFactory _exceptionMessageFormatterFactory = new ExceptionMessageFormatterFactory(
            new IExceptionMessageFormatter []
            {
                new ExceptionMessageFormatter()
            });

        public static void UseExceptionMessageFormatterFactory(
            IExceptionMessageFormatterFactory exceptionMessageFormatterFactory)
        {
            _exceptionMessageFormatterFactory = exceptionMessageFormatterFactory;
        }

        public static string GetMessage(this Exception exception)
        {
            const int reasonableInitialMessageSize = 200;
            var messageBuilder = new StringBuilder(reasonableInitialMessageSize);

            GetExceptionMessage(exception, messageBuilder);

            return messageBuilder.ToString();
        }

        public static void AppendMessage(this Exception exception, StringBuilder messageBuilder)
        {
            GetExceptionMessage(exception, messageBuilder);
        }

        private static void GetExceptionMessage(Exception exception, StringBuilder messageBuilder)
        {
            var messageFormatter = _exceptionMessageFormatterFactory.GetFormatter(exception);

            try
            {
                messageFormatter.AppendFormattedMessage(exception, messageBuilder);
            }
            catch (Exception e)
            {
                messageBuilder.Append("[An exception occurred whilst formatting the exception ");
                messageBuilder.Append(exception.GetType().Name);
                messageBuilder.Append(' ');
                messageBuilder.Append(exception.Message);
                messageBuilder.Append("]. The exception that occurred was ");
                messageBuilder.Append(e.GetType().Name);
                messageBuilder.Append(' ');
                messageBuilder.Append(e.Message);
                messageBuilder.AppendLine();
            }
        }
    }
}