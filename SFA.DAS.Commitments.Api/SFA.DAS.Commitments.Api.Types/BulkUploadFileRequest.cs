namespace SFA.DAS.Commitments.Api.Types
{
    public class BulkUploadFileRequest
    {
        public long CommitmentId { get; set; }

        public string FileName { get; set; }

        public string Data { get; set; }
    }
}