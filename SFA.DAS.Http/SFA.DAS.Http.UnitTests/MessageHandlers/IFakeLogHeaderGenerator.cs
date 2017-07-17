using SFA.DAS.Http.MessageHandlers;

namespace SFA.DAS.Http.UnitTests.MessageHandlers
{
    public class IFakeLogHeaderGenerator : IGenerateRequestHeader
    {
        public string Name {
            get { return "HeaderName"; }
            set {}
        }

        public string Generate()
        {
            return "log-value";
        }
    }
}