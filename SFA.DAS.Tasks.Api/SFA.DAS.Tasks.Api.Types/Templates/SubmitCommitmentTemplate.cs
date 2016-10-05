namespace SFA.DAS.Tasks.Api.Types.Templates
{
    public class SubmitCommitmentTemplate
    {
        public const long TemplateId = 2;

        public long CommitmentId { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }
}