using System.Collections.Generic;

namespace SFA.DAS.Notifications.Api.Types
{
    public class Email
    {
        public string SystemId { get; set; }
        public string TemplateId { get; set; }
        public string Subject { get; set; }
        public string RecipientsAddress { get; set; }
        public string ReplyToAddress { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}