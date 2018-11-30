namespace SFA.DAS.VacancyServices.Search
{
    using Nest;

    internal interface IElasticSearchFactory
    {
        IElasticClient GetElasticClient(string hostName);
    }
}