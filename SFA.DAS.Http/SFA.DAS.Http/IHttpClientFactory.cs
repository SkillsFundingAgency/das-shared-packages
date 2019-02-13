using System.Net.Http;

namespace SFA.DAS.Http
{
    public interface IHttpClientFactory
    {
        HttpClient CreateHttpClient();
    }
}