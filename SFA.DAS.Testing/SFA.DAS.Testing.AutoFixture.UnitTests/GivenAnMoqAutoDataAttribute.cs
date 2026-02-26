using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Testing.AutoFixture.UnitTests
{
    [TestFixture]
    public class GivenAnMoqAutoDataAttribute
    {
        [Test, MoqAutoData]
        public void Then_Injects_Mocked_Interface(
            [Frozen] Mock<IDependency> mockDependency,
            Consumer consumer)
        {
            Assert.Pass();
        }
    }

    public interface IDependency
    {
            
    }

    public class Consumer
    {
        public Consumer(IDependency dependency)
        {
                
        }
    }
}