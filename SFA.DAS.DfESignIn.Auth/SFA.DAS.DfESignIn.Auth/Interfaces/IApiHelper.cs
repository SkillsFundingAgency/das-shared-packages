using System;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface IApiHelper
    {
        /// <summary>
        /// Property to get and set Bearer Token.
        /// </summary>
        string AccessToken { get; set; }

        /// <summary>
        /// Method to make GET call for a HttpClient.
        /// </summary>
        /// <typeparam name="T">TResult.</typeparam>
        /// <param name="endpoint">Destination endpoint.</param>
        /// <returns>TResult.</returns>
        /// <exception cref="MemberAccessException"></exception>
        Task<T> Get<T>(string endpoint);
    }
}
