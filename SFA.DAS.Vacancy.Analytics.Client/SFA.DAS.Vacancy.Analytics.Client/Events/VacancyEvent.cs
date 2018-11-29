using System;

namespace Esfa.Vacancy.Analytics.Events
{
    public abstract class VacancyEvent
    {
        public long VacancyReference { get; protected set; }
        public abstract string EventType { get; }
        public abstract DateTime EventTime { get; protected set; }
        public abstract string PublisherId { get; protected set; }
    }
}