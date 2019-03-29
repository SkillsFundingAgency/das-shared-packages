using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Http
{
    public interface IRestHttpClient
    {
        Task<string> Get(Uri uri, object queryData = null, CancellationToken cancellationToken = default);
        Task<string> Get(string uri, object queryData = null, CancellationToken cancellationToken = default);
        Task<T> Get<T>(Uri uri, object queryData = null, CancellationToken cancellationToken = default);
        Task<T> Get<T>(string uri, object queryData = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Serialize the supplied requestData into json and post to the specified URI.
        /// </summary>
        /// <returns>
        ///     The content of the response as a string.
        /// </returns>
        Task<string> PostAsJson<TRequest>(string uri, TRequest requestData, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Serialize the supplied requestData into json and post to the specified URI.
        /// </summary>
        /// <returns>
        ///     The content of the response de-serialized into an instance of TResponse.
        /// </returns>
        /// 
        Task<TResponse> PostAsJson<TRequest, TResponse>(string uri, TRequest requestData, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Serialize the supplied requestData into json and PUT to the specified URI.
        /// </summary>
        /// <returns>
        ///     The content of the response as a string.
        /// </returns>
        Task<string> PutAsJson<TRequest>(string uri, TRequest requestData, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Serialize the supplied requestData into json and PUT to the specified URI.
        /// </summary>
        /// <returns>
        ///     The content of the response de-serialized into an instance of TResponse.
        /// </returns>
        Task<TResponse> PutAsJson<TRequest, TResponse>(string uri, TRequest requestData, CancellationToken cancellationToken = default);
    }
}