using System;
using System.Net.Http;
using System.Text;

namespace SFA.DAS.Logging.MessageFormatters
{
    internal class HttpRequestExceptionMessageFormatter : BaseExceptionMessageFormatter
    {
        public override Type SupportedException => typeof(HttpRequestException);

        protected override void CreateFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            var httpException = (HttpRequestException)exception;

            messageBuilder.AppendLine($"Exception: {httpException.GetType().Name}");
            messageBuilder.AppendLine($"Message: {httpException.Message}");

            exception.InnerException?.AppendMessage(messageBuilder);
        }
    }
}
