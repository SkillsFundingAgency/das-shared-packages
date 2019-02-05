using System;
using NUnit.Framework;
using SFA.DAS.ExceptionHandling.MessageFormatters;
using System.Collections.Generic;

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
        public void GetFormatter_ForException_ShouldGetExceptionFormatter(Type exceptionType, Type expectedFormatterType)
        {
            // Arrange
            var fixtures = new ExceptionMessageFormatterFactoryTestFixtures();
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
        public ExceptionMessageFormatterFactoryTestFixtures()
        {
            
        }

        public List<ExceptionMessageFormatter> ExceptionMessageFormatters { get; } = new List<ExceptionMessageFormatter>();

        public ExceptionMessageFormatterFactory CreateFactory()
        {
            return new ExceptionMessageFormatterFactory(ExceptionMessageFormatters.ToArray());
        }
    }
}
