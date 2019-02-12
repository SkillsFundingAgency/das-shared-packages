using System;
using NUnit.Framework;
using SFA.DAS.ExceptionHandling.MessageFormatters;
using System.Collections.Generic;
using System.Net.Http;

namespace SFA.DAS.ExceptionHandling.UnitTests
{
    [TestFixture]
    public class AggregateExceptionMessageFormatterTests
    {
        [Test]
        public void GetFormattedMessage_MoreThanOneInnerException_ShouldReturnMessages()
        {
            const string ex1 = "EX1";
            const string ex2 = "EX2";

            // Arrange
            var exception = new AggregateException(new Exception(ex1), new Exception(ex2));
            var formatter = new AggregateExceptionMessageFormatter();

            // Act
            var msg = formatter.GetFormattedMessage(exception);

            // Assert
            Assert.IsTrue(msg.Contains(ex1));
            Assert.IsTrue(msg.Contains(ex2));
        }

        [Test]
        public void GetFormattedMessage_MessageButNoInnerException_ShouldReturnMessages()
        {
            string message = Guid.NewGuid().ToString();

            // Arrange
            var exception = new AggregateException(message);
            var formatter = new AggregateExceptionMessageFormatter();

            // Act
            var msg = formatter.GetFormattedMessage(exception);

            // Assert
            Assert.IsTrue(msg.Contains(message));
        }

        [Test]
        public void GetFormattedMessage_NoMessageAndNoInnerException_ShouldReturnMessages()
        {
            // Arrange
            var exception = new AggregateException();
            var formatter = new AggregateExceptionMessageFormatter();

            // Act
            var msg = formatter.GetFormattedMessage(exception);

            // Assert
            Assert.Pass("Handled exception without exception");
        }
    }
}
