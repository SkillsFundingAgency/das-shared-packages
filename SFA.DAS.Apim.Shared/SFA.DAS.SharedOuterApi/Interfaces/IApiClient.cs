using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.SharedOuterApi.Models;

namespace SFA.DAS.SharedOuterApi.Interfaces;

public interface IApiClient<T> : IGetApiClient<T>
{
    Task<IEnumerable<TResponse>> GetAll<TResponse>(IGetAllApiRequest request);
    Task<PagedResponse<TResponse>> GetPaged<TResponse>(IGetPagedApiRequest request);
    [Obsolete("Use PostWithResponseCode")]
    Task<TResponse> Post<TResponse>(IPostApiRequest request);
    [Obsolete("Use PostWithResponseCode")]
    Task Post<TData>(IPostApiRequest<TData> request);
    Task Delete(IDeleteApiRequest request);

    Task<ApiResponse<TResponse>> DeleteWithResponseCode<TResponse>(IDeleteApiRequest request, bool includeResponse = false);

    Task Patch<TData>(IPatchApiRequest<TData> request);
    Task Put(IPutApiRequest request);
    Task Put<TData>(IPutApiRequest<TData> request);
    Task<ApiResponse<TResponse>> PostWithResponseCode<TResponse>(IPostApiRequest request, bool includeResponse = true);
    Task<ApiResponse<TResponse>> PostWithResponseCode<TData, TResponse>(IPostApiRequest<TData> request, bool includeResponse = true)
    {
        throw new System.NotImplementedException();
    }
    Task<ApiResponse<string>> PatchWithResponseCode<TData>(IPatchApiRequest<TData> request);
    Task<ApiResponse<TResponse>> PatchWithResponseCode<TData, TResponse>(IPatchApiRequest<TData> request, bool includeResponse = true);
    /// <summary>
    /// Sends a PUT request to an API endpoint and returns a response code.
    /// </summary>
    /// <typeparam name="TResponse">If the API returns no data use NullResponse type.</typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse<TResponse>> PutWithResponseCode<TResponse>(IPutApiRequest request) where TResponse : class;
    Task<ApiResponse<TResponse>> PutWithResponseCode<TData, TResponse>(IPutApiRequest<TData> request);
}
