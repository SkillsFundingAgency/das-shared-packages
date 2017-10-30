

namespace NuGet.Messages
{
    public class ApprenticeChangesRequestedMessage: Message
    {
        public ApprenticeChangesRequestedMessage()
        {

        }

        public ApprenticeChangesRequestedMessage(string apprenticeName)
        {
            ApprenticeName = apprenticeName;
        }

        public string ApprenticeName { get; }
    }
}
