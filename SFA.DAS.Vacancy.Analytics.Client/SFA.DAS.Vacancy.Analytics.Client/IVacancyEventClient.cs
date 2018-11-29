using System.Threading.Tasks;
using Esfa.Vacancy.Analytics.Events;

namespace Esfa.Vacancy.Analytics
{
    public interface IVacancyEventClient
    {
        Task PushApprenticeshipSearchEventAsync(long vacancyReference);
        Task PushApprenticeshipDetailEventAsync(long vacancyReference);
        Task PushApprenticeshipBookmarkedEventAsync(long vacancyReference);
        Task PushApprenticeshipSavedSearchAlertEventAsync(long vacancyReference);

        Task PushApprenticeshipApplicationCreatedEventAsync(long vacancyReference);
        Task PushApprenticeshipApplicationSubmittedEventAsync(long vacancyReference);
    }
}