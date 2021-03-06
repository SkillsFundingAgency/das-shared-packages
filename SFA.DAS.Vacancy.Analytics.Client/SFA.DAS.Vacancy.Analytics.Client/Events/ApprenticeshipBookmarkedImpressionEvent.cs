using System;

namespace Esfa.Vacancy.Analytics.Events
{
    public sealed class ApprenticeshipBookmarkedImpressionEvent : VacancyEvent
    {
        public override string EventType => this.GetType().Name;
        public override DateTime EventTime { get; set; }
        public override string PublisherId { get; set; }

        public ApprenticeshipBookmarkedImpressionEvent(long vacancyReference)
        {
            VacancyReference = vacancyReference;
			EventTime = DateTime.UtcNow;
        }

		internal ApprenticeshipBookmarkedImpressionEvent(long vacancyReference, string publisherId) : this(vacancyReference)
        {
            PublisherId = publisherId;
        }
    }
}