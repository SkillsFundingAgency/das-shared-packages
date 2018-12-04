using System;

namespace Esfa.Vacancy.Analytics.Events
{
    public abstract class VacancyEvent
    {
        public Guid Id { get; set; }
        public long VacancyReference { get; protected set; }
        public abstract string EventType { get; }
        public abstract DateTime EventTime { get; protected set; }
        public abstract string PublisherId { get; protected set; }

        public VacancyEvent()
        {
            Id = Guid.NewGuid();
        }
    }
}