using System;
using NUnit.Framework;
using SFA.DAS.ExceptionHandling.MessageFormatters;
using System.Collections.Generic;
using System.Net.Http;

namespace SFA.DAS.ExceptionHandling.UnitTests
{
    [TestFixture]
    public class ExceptionMessageFormatterFactoryTests
    {
        [Test]
        public void Constructor_ValidCall_ShouldNotThrowException()
        {
            // Arrange
            var fixtures = new ExceptionMessageFormatterFactoryTestFixtures();

            // Act
            var factory = fixtures.CreateFactory();

            // Assert
            Assert.Pass("The factory was created successfully");
        }

        [TestCase(typeof(Exception), typeof(ExceptionMessageFormatter))]
        [TestCase(typeof(HttpRequestException), typeof(HttpRequestExceptionMessageFormatter))]
        [TestCase(typeof(AggregateException), typeof(AggregateExceptionMessageFormatter))]
        // No specific exception handler for this, so should default
        [TestCase(typeof(InvalidCastException), typeof(ExceptionMessageFormatter))]
        public void GetFormatter_ForException_ShouldGetExceptionFormatter(Type exceptionType, Type expectedFormatterType)
        {
            // Arrange
            var fixtures = new ExceptionMessageFormatterFactoryTestFixtures()
                .WithExceptionFormatter(new HttpRequestExceptionMessageFormatter())
                .WithExceptionFormatter(new AggregateExceptionMessageFormatter());

            var factory = fixtures.CreateFactory();
            var exception = Activator.CreateInstance(exceptionType) as Exception;

            // Actual
            var actualFormatterType = factory.GetFormatter(exception);

            // Assert
            Assert.AreEqual(expectedFormatterType, actualFormatterType.GetType());
        }
    }

    public class ExceptionMessageFormatterFactoryTestFixtures
    {
        public List<IExceptionMessageFormatter> ExceptionMessageFormatters { get; } = new List<IExceptionMessageFormatter>();

        public ExceptionMessageFormatterFactoryTestFixtures WithExceptionFormatter(IExceptionMessageFormatter formatter)
        {
            ExceptionMessageFormatters.Add(formatter);
            return this;
        }

        public ExceptionMessageFormatterFactory CreateFactory()
        {
            return new ExceptionMessageFormatterFactory(ExceptionMessageFormatters.ToArray());
        }
    }
}
