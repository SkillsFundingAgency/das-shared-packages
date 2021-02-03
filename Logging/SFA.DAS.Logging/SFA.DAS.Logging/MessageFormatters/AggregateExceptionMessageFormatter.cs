using System;
using System.Text;

namespace SFA.DAS.Logging.MessageFormatters
{
    internal class AggregateExceptionMessageFormatter : BaseExceptionMessageFormatter
    {
        public override Type SupportedException => typeof(AggregateException);

        protected override void CreateFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            var exceptions = ((AggregateException)exception).Flatten();

            messageBuilder.AppendLine($"Aggregate exception has {exceptions.InnerExceptions.Count} inner exception.");

            var exceptionCount = 1;

            foreach (var ex in exceptions.InnerExceptions)
            {
                messageBuilder.AppendLine($"Exception {exceptionCount}: ");

                ex.AppendMessage(messageBuilder);

                ex.InnerException?.AppendMessage(messageBuilder);

                exceptionCount++;
            }
        }
    }
}
