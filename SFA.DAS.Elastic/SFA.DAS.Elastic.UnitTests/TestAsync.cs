using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.Elastic.UnitTests
{
    public abstract class TestAsync
    {
        [OneTimeSetUp]
        public async Task SetUp()
        {
            Given();
            await When();
        }

        protected abstract void Given();
        protected abstract Task When();
    }
}