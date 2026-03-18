using System;
using SFA.DAS.SharedOuterApi.Exceptions;

namespace SFA.DAS.SharedOuterApi.UnitTests.Exceptions
{
    public class WhenConstructingNotFoundException
    {
        [Test]
        public void And_Using_Default_Ctor_Then_Sets_Message_From_T()
        {
            var exception = new NotFoundException<TestType>();

            exception.Message.Should().Be($"[{nameof(TestType)}] cannot be found");
        }

        [Test, AutoData]
        public void And_Using_Message_Ctor_Then_Sets_Message_From_Param(
            string message)
        {
            var exception = new NotFoundException<TestType>(message);

            exception.Message.Should().Be(message);
        }

        [Test, AutoData]
        public void And_Using_InnerException_Ctor_Then_Sets_Message_From_T(
            Exception innerException)
        {
            var exception = new NotFoundException<TestType>(innerException);

            exception.Message.Should().Be($"[{nameof(TestType)}] cannot be found");
            exception.InnerException.Should().Be(innerException);
        }

        [Test, AutoData]
        public void And_Using_Message_And_InnerException_Ctor_Then_Sets_Message_From_Param(
            string message,
            Exception innerException)
        {
            var exception = new NotFoundException<TestType>(message, innerException);

            exception.Message.Should().Be(message);
            exception.InnerException.Should().Be(innerException);
        }
    }

    public class TestType
    {
    }
}