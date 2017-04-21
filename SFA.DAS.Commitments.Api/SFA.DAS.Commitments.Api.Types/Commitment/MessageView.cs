using System;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.Commitments.Api.Types.Commitment
{
    public class MessageView
    {
        public string Author { get; set; }
        public string Message { get; set; }
        public MessageCreator CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
