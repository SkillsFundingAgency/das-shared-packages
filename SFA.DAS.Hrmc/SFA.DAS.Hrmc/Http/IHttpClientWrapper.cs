using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.Hrmc.Http
{
    public interface IHttpClientWrapper
    {
        string BaseUrl { get; set; }
        List<MediaTypeWithQualityHeaderValue> MediaTypeWithQualityHeaderValueList { get; set; }
        string AuthScheme { get; set; }
        Task<string> SendMessage<T>(T content, string url);
        Task<T> Get<T>(string authToken, string url);
        Task<string> GetString(string url, string accessToken);
    }
}