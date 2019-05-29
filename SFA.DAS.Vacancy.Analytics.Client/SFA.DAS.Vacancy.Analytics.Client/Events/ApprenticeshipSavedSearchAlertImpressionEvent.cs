using System;

namespace Esfa.Vacancy.Analytics.Events
{
    public sealed class ApprenticeshipSavedSearchAlertImpressionEvent : VacancyEvent
    {
        public override string EventType => this.GetType().Name;
        public override DateTime EventTime { get; set; }
        public override string PublisherId { get; set; }

		public ApprenticeshipSavedSearchAlertImpressionEvent(long vacancyReference)
        {
            VacancyReference = vacancyReference;
            EventTime = DateTime.UtcNow;
        }

        internal ApprenticeshipSavedSearchAlertImpressionEvent(long vacancyReference, string publisherId) : this(vacancyReference)
        {
            PublisherId = publisherId;
        }
    }
}