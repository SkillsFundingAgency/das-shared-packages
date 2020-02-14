using Nest;

namespace SFA.DAS.VacancyServices.Search.Clients
{
    public interface IElasticSearchClientFactory
    {
        IElasticClient GetElasticClient();
    }
}
