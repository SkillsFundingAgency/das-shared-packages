using SFA.DAS.SharedOuterApi.Interfaces;

namespace SFA.DAS.SharedOuterApi.UnitTests.Infrastructure.InternalApi
{
    public class TestInternalApiConfiguration : IInternalApiConfiguration
    {
        public virtual string Url { get; set; }
        public virtual string Identifier { get; set; }
    }
}