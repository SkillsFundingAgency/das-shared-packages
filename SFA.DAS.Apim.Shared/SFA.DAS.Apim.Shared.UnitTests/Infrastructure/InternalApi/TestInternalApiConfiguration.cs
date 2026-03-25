using SFA.DAS.Apim.Shared.Interfaces;

namespace SFA.DAS.Apim.Shared.UnitTests.Infrastructure.InternalApi
{
    public class TestInternalApiConfiguration : IInternalApiConfiguration
    {
        public virtual string Url { get; set; }
        public virtual string Identifier { get; set; }
    }
}