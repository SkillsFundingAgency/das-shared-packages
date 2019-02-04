using System;
using System.Data.SqlClient;
using System.Text;

namespace SFA.DAS.ExceptionHandling.MessageFormatters
{
    internal class SqlExceptionMessageFormatter : BaseExceptionMessageFormatter
    {
        public override Type SupportedException => typeof(SqlException);

        protected override void CreateFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            var sqlException = (SqlException)exception;

            messageBuilder.AppendLine($"Exception: {sqlException.GetType().Name}");
            messageBuilder.AppendLine($"Message: {sqlException.Message}");
            messageBuilder.AppendLine($"Procedure: {sqlException.Procedure} LineNumber:{sqlException.LineNumber} State:{sqlException.State} Error:{sqlException.Number}");

            foreach (SqlError error in sqlException.Errors)
            {
                messageBuilder.AppendLine($"Message:{error.Message} Procedure: {error.Procedure} LineNumber:{error.LineNumber} State:{error.State} Error:{error.Number}");
            }

            exception.InnerException?.AppendMessage(messageBuilder);
        }
    }
}
