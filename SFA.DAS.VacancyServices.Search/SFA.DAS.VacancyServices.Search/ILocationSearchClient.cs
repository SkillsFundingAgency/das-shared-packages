namespace SFA.DAS.VacancyServices.Search
{
    using Responses;

    public interface ILocationSearchClient
    {
        LocationSearchResponse Search(string placeName, int maxResults = LocationSearchClient.MaxResults);
    }
}