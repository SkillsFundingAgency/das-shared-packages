using System;

namespace SFA.DAS.ExceptionHandling
{

    /// <summary>
    ///     Provides a formatter for a given exception.
    /// </summary>
    public interface IExceptionMessageFormatterFactory
    {
        /// <summary>
        ///     The most specific formatter will be provided. If there is not a formatter for the
        ///     specific exception type then the one for parent class will be used. 
        /// </summary>
        IExceptionMessageFormatter GetFormatter(Exception exception);
    }
}