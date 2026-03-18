using System;
using System.Net;
using SFA.DAS.Apim.Shared.Exceptions;
using SFA.DAS.Apim.Shared.Models;

namespace SFA.DAS.Apim.Shared.Extensions
{
    public static class ApiResponseErrorChecking
    {
        public static ApiResponse<T> EnsureSuccessStatusCode<T>(this ApiResponse<T> response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (!IsSuccessStatusCode(response.StatusCode))
            {
                throw ApiResponseException.Create(response);
            }

            return response;
        }

        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
            => (int)statusCode >= 200 && (int)statusCode <= 299;
    }
}