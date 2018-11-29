using System;

namespace Esfa.Vacancy.Analytics.Events
{
    public sealed class ApprenticeshipApplicationSubmittedEvent : VacancyEvent
    {
        public override string EventType => this.GetType().Name;
        public override DateTime EventTime { get; protected set; }
        public override string PublisherId { get; protected set; }

        public ApprenticeshipApplicationSubmittedEvent(long vacancyReference, string publisherId)
        {
            VacancyReference = vacancyReference;
            PublisherId = publisherId;
            EventTime = DateTime.UtcNow;
        }
    }
}