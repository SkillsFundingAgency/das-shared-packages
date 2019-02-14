using System;
using NUnit.Framework;
using SFA.DAS.ExceptionHandling.MessageFormatters;
using System.Collections.Generic;
using System.Net.Http;

namespace SFA.DAS.ExceptionHandling.UnitTests
{
    [TestFixture]
    public class ExceptionMessageFormatterTests
    {
        [Test]
        public void GetFormattedMessage_WithMessageAndNoInnerException_ShouldReturnMessage()
        {
            string message = Guid.NewGuid().ToString();

            // Arrange
            var exception = new Exception(message);
            var formatter = new ExceptionMessageFormatter();

            // Act
            var msg = formatter.GetFormattedMessage(exception);

            // Assert
            Assert.IsTrue(msg.Contains(message));
        }

        [Test]
        public void GetFormattedMessage_WitInnerException_ShouldReturnMessage()
        {
            // Arrange
            var exception = new Exception(string.Empty, new AggregateException());
            var formatter = new ExceptionMessageFormatter();

            // Act
            var msg = formatter.GetFormattedMessage(exception);

            // Assert
            Assert.Pass("Handled exception without exception");
        }
    }
}
