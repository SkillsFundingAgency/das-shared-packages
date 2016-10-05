namespace SFA.DAS.Tasks.Api.Types.Templates
{
    public class CreateCommitmentTemplate
    {
        public const long TemplateId = 1;

        public long CommitmentId { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }
}