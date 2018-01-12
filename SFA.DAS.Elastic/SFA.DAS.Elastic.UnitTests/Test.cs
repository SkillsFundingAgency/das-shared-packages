using NUnit.Framework;

namespace SFA.DAS.Elastic.UnitTests
{
    public abstract class Test
    {
        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Given();
            When();
        }

        protected abstract void Given();
        protected abstract void When();
    }
}