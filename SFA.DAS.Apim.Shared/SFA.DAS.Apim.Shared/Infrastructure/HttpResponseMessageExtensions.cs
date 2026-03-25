using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Apim.Shared.Infrastructure
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task EnsureSuccessStatusCodeIncludeContentInException(this HttpResponseMessage response)
        {
            string? errorContent = null;

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                catch
                {
                    // do nothing
                }

                throw new HttpRequestContentException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode})", response.StatusCode, errorContent);
            }
        }
    }
}