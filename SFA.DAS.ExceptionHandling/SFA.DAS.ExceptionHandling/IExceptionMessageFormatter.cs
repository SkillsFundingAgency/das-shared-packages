using System;
using System.Text;

namespace SFA.DAS.ExceptionHandling
{

    /// <summary>
    ///     Represents a formatter that can compose the most useful message from a
    ///     specific exception type.
    /// </summary>
    public interface IExceptionMessageFormatter
    {
        /// <summary>
        ///     The type of exception that this formatter can handle. A formatter may also
        ///     be asked to handle child exception types if there are no more specific
        ///     handlers available.
        /// </summary>
        Type SupportedException { get; }

        /// <summary>
        ///     Returns a string representing this exception. The formatter may choose to include
        ///     inner exception information.
        /// </summary>
        string GetFormattedMessage(Exception exception);


        /// <summary>
        ///     The same as <see cref="GetFormattedMessage"/> but the message will be appended
        ///     to the supplied string builder.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="messageBuilder"></param>
        void AppendFormattedMessage(Exception exception, StringBuilder messageBuilder);
    }
}
