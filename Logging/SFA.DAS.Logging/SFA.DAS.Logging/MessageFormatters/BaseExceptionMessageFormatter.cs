using System;
using System.Text;

namespace SFA.DAS.Logging.MessageFormatters
{
    internal abstract class BaseExceptionMessageFormatter : IExceptionMessageFormatter
    {
        public virtual Type SupportedException => typeof(Exception);

        public string GetFormattedMessage(Exception exception)
        {
            var messageBuilder = new StringBuilder();

            AppendFormattedMessage(exception, messageBuilder);

            return messageBuilder.ToString();
        }

        public void AppendFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            CreateFormattedMessage(exception, messageBuilder);
        }

        protected abstract void CreateFormattedMessage(Exception exception, StringBuilder messageBuilder);
    }
}
