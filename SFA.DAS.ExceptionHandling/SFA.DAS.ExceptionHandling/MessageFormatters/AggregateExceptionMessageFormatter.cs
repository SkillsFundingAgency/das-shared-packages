using System;
using System.Text;

namespace SFA.DAS.ExceptionHandling.MessageFormatters
{
    public class AggregateExceptionMessageFormatter : BaseExceptionMessageFormatter
    {
        public override Type SupportedException => typeof(AggregateException);

        protected override void CreateFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            var aggregateException = (AggregateException) exception;

            var exceptions = aggregateException.Flatten();

            messageBuilder.AppendLine($"Aggregate exception has {exceptions.InnerExceptions.Count} inner exception.");

            if (!string.IsNullOrEmpty(aggregateException.Message))
            {
                messageBuilder.AppendLine(aggregateException.Message);
            }

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
