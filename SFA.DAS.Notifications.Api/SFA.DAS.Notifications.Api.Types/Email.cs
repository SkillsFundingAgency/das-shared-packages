using System.Collections.Generic;

namespace SFA.DAS.Notifications.Api.Types
{
    public class Email
    {
        /// <summary>
        /// TODO
        /// </summary>
        public string SystemId { get; set; }
        /// <summary>
        /// TODO
        /// </summary>
        public string TemplateId { get; set; }
        /// <summary>
        /// TODO
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// TODO
        /// </summary>
        public string RecipientsAddress { get; set; }
        /// <summary>
        /// TODO
        /// </summary>
        public string ReplyToAddress { get; set; }
        /// <summary>
        /// TODO
        /// </summary>
        public Dictionary<string, string> Tokens { get; set; }
    }
}