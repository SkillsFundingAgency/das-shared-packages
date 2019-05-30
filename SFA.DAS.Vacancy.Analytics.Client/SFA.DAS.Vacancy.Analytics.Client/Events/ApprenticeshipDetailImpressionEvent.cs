using System;

namespace Esfa.Vacancy.Analytics.Events
{
	public sealed class ApprenticeshipDetailImpressionEvent : VacancyEvent
	{
		public override string EventType => this.GetType().Name;
		public override DateTime EventTime { get; set; }
		public override string PublisherId { get; set; }

		public ApprenticeshipDetailImpressionEvent(long vacancyReference)
		{
			VacancyReference = vacancyReference;
			EventTime = DateTime.UtcNow;
		}

		internal ApprenticeshipDetailImpressionEvent(long vacancyReference, string publisherId) : this(vacancyReference)
		{
			PublisherId = publisherId;
			EventTime = DateTime.UtcNow;
		}
	}
}