using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public class TrainingCourse
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public TrainingType TrainingType { get; set; }
    }
}