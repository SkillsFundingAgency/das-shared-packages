using System;

namespace Esfa.Vacancy.Analytics.Events
{
    public sealed class ApprenticeshipApplicationCreatedEvent : VacancyEvent
{
    public override string EventType => this.GetType().Name;
    public override DateTime EventTime { get; protected set; }
    public override string PublisherId { get; protected set; }

    public ApprenticeshipApplicationCreatedEvent(long vacancyReference, string publisherId)
    {
        VacancyReference = vacancyReference;
        PublisherId = publisherId;
        EventTime = DateTime.UtcNow;
    }
}
}