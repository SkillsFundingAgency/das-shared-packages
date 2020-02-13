namespace SFA.DAS.VacancyServices.Search
{
    using Nest;

    internal interface IElasticSearchFactory
    {
        IElasticClient GetElasticClient(ApprenticeshipSearchClientConfiguration config);

        IElasticClient GetElasticClient(TraineeshipSearchClientConfiguration config);

        IElasticClient GetElasticClient(LocationSearchClientConfiguration config);

    }
}