using System;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.Commitments.Api.Types.DataLock
{
    public class DataLockStatus
    {
        public long DataLockEventId { get; set; }
        public DateTime DataLockEventDatetime { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public long ApprenticeshipId { get; set; }
        public string IlrTrainingCourseCode { get; set; }
        public TrainingType IlrTrainingType { get; set; }
        public DateTime? IlrActualStartDate { get; set; }
        public DateTime? IlrEffectiveFromDate { get; set; }
        public decimal? IlrTotalCost { get; set; }
        public Status Status { get; set; }
        public TriageStatus TriageStatus { get; set; }
        public DataLockErrorCode ErrorCode { get; set; }
        public long ApprenticeshipUpdateId { get; set; }
        public bool IsResolved { get; set; }
    }
}
