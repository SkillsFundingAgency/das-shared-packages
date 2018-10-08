using System;

namespace SFA.DAS.Recruit.Vacancies.Client.Messages
{
    public class ApplicationWithdrawMessage
    {
        public Guid CandidateId { get; set; }
        public long VacancyReference { get; set; }
    }
}
