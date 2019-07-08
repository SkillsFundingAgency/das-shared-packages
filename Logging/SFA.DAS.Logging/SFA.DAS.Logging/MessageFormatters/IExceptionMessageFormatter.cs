using System;
using System.Text;

namespace SFA.DAS.Logging.MessageFormatters
{
    public interface IExceptionMessageFormatter
    {
        Type SupportedException { get; }
        string GetFormattedMessage(Exception exception);
        void AppendFormattedMessage(Exception exception, StringBuilder messageBuilder);
    }
}
