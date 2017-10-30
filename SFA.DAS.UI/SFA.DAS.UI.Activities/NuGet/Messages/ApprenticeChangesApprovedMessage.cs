

namespace NuGet.Messages
{
    public class ApprenticeChangesApprovedMessage : Message
    {
        public ApprenticeChangesApprovedMessage()
        {

        }

        public ApprenticeChangesApprovedMessage(string apprenticeName)
        {
            ApprenticeName = apprenticeName;
        }

        public string ApprenticeName { get;  }
    }
}
