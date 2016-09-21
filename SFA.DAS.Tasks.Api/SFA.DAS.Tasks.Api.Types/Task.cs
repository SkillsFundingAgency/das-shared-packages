using System;

namespace SFA.DAS.Tasks.Api.Types
{
    public class Task
    {
        public long Id { get; set; }
        public string Assignee { get; set; }
        public long TaskTemplateId { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public TaskStatuses TaskStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string CompletedBy { get; set; }
    }
}