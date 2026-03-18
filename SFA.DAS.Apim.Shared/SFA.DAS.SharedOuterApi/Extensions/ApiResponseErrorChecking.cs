using SFA.DAS.SharedOuterApi.Exceptions;
using System;
using System.Net;
using SFA.DAS.SharedOuterApi.Models;

namespace SFA.DAS.SharedOuterApi.Extensions
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