using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Apim.Shared.Interfaces;
using SFA.DAS.Apim.Shared.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SFA.DAS.Apim.Shared.Infrastructure;

public abstract class ApiClient<T> : GetApiClient<T>, IApiClient<T> where T : IApiConfiguration
{
    public static readonly JsonSerializerOptions JsonSerializationOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(
        IHttpClientFactory httpClientFactory,
        T apiConfiguration) : base(httpClientFactory, apiConfiguration)
    {
    }

    [Obsolete("Use PostWithResponseCode")]
    public async Task<TResponse> Post<TResponse>(IPostApiRequest request)
    {
        var result = await PostWithResponseCode<TResponse>(request);

        if (IsNot200RangeResponseCode(result.StatusCode))
        {
            throw new HttpRequestContentException($"Response status code does not indicate success: {(int)result.StatusCode} ({result.StatusCode})", result.StatusCode, result.ErrorContent);
        }

        return result.Body;
    }

    public async Task<ApiResponse<TResponse>> PostWithResponseCode<TResponse>(IPostApiRequest request, bool includeResponse = true)
    {
        return await PostWithResponseCode<object, TResponse>(request, includeResponse);
    }

    public async Task<ApiResponse<TResponse>> PostWithResponseCode<TData, TResponse>(IPostApiRequest<TData> request, bool includeResponse = true)
    {
        var stringContent = request.Data != null ? new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json") : null;

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, request.PostUrl);
        requestMessage.AddVersion(request.Version);
        requestMessage.Content = stringContent;
        await AddAuthenticationHeader(requestMessage);
        requestMessage.AddCorrelationId();

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var errorContent = "";
        var responseBody = (TResponse)default;

        if (IsNot200RangeResponseCode(response.StatusCode))
        {
            errorContent = json;
            HandleException(response, json);
        }
        else if (includeResponse && !string.IsNullOrWhiteSpace(json))
        {            
            responseBody = JsonSerializer.Deserialize<TResponse>(json, JsonSerializationOptions);
        }

        var postWithResponseCode = new ApiResponse<TResponse>(responseBody, response.StatusCode, errorContent);

        return postWithResponseCode;
    }

    public virtual string HandleException(HttpResponseMessage response, string json)
    {
        return json;
    }

    [Obsolete("Use PostWithResponseCode")]
    public async Task Post<TData>(IPostApiRequest<TData> request)
    {
        var stringContent = request.Data != null ? new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json") : null;

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, request.PostUrl);
        requestMessage.AddVersion(request.Version);
        requestMessage.Content = stringContent;
        await AddAuthenticationHeader(requestMessage);

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);

        await response.EnsureSuccessStatusCodeIncludeContentInException();
    }

    public async Task Delete(IDeleteApiRequest request)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, request.DeleteUrl);
        requestMessage.AddVersion(request.Version);
        await AddAuthenticationHeader(requestMessage);
        requestMessage.AddCorrelationId();

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);

        await response.EnsureSuccessStatusCodeIncludeContentInException();
    }

    public async Task<ApiResponse<TResponse>> DeleteWithResponseCode<TResponse>(IDeleteApiRequest request, bool includeResponse = false)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, request.DeleteUrl);
        requestMessage.AddVersion(request.Version);
        await AddAuthenticationHeader(requestMessage);
        requestMessage.AddCorrelationId();

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var errorContent = string.Empty;
        var responseBody = (TResponse)default;

        if (IsNot200RangeResponseCode(response.StatusCode))
        {
            errorContent = json;
            HandleException(response, json);
        }
        else if (includeResponse)
        {
            responseBody = JsonSerializer.Deserialize<TResponse>(json, JsonSerializationOptions);
        }

        var patchWithResponseCode = new ApiResponse<TResponse>(responseBody, response.StatusCode, errorContent);

        return patchWithResponseCode;
    }

    [Obsolete("Use PatchWithResponseCode")]
    public async Task Patch<TData>(IPatchApiRequest<TData> request)
    {
        var stringContent = request.Data != null ? new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json") : null;
        var requestMessage = new HttpRequestMessage(HttpMethod.Patch, request.PatchUrl);
        requestMessage.AddVersion(request.Version);
        requestMessage.Content = stringContent;
        await AddAuthenticationHeader(requestMessage);

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);

        await response.EnsureSuccessStatusCodeIncludeContentInException();
    }


    public async Task<ApiResponse<TResponse>> PatchWithResponseCode<TData, TResponse>(IPatchApiRequest<TData> request, bool includeResponse = true)
    {
        var stringContent = request.Data != null ? new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json") : null;

        var requestMessage = new HttpRequestMessage(HttpMethod.Patch, request.PatchUrl);
        requestMessage.AddVersion(request.Version);
        requestMessage.Content = stringContent;
        await AddAuthenticationHeader(requestMessage);
        requestMessage.AddCorrelationId();

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var errorContent = "";
        var responseBody = (TResponse)default;

        if (IsNot200RangeResponseCode(response.StatusCode))
        {
            errorContent = json;
            HandleException(response, json);
        }
        else if (includeResponse)
        {
            responseBody = JsonSerializer.Deserialize<TResponse>(json, JsonSerializationOptions);
        }

        var patchWithResponseCode = new ApiResponse<TResponse>(responseBody, response.StatusCode, errorContent);

        return patchWithResponseCode;
    }

    public async Task<ApiResponse<string>> PatchWithResponseCode<TData>(IPatchApiRequest<TData> request)
    {
        var stringContent = request.Data != null ? new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json") : null;
        var requestMessage = new HttpRequestMessage(HttpMethod.Patch, request.PatchUrl);
        requestMessage.AddVersion(request.Version);
        requestMessage.Content = stringContent;
        await AddAuthenticationHeader(requestMessage);
        requestMessage.AddCorrelationId();

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return new ApiResponse<string>(responseContent, response.StatusCode, ""); //TODO - Error content should be correctly set
    }

    public async Task Put(IPutApiRequest request)
    {
        var stringContent = request.Data != null ? new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json") : null;
        var requestMessage = new HttpRequestMessage(HttpMethod.Put, request.PutUrl);
        requestMessage.AddVersion(request.Version);
        requestMessage.Content = stringContent;
        await AddAuthenticationHeader(requestMessage);
        requestMessage.AddCorrelationId();

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        await response.EnsureSuccessStatusCodeIncludeContentInException();
    }

    public async Task Put<TData>(IPutApiRequest<TData> request)
    {
        var stringContent = request.Data != null ? new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json") : null;
        var requestMessage = new HttpRequestMessage(HttpMethod.Put, request.PutUrl);
        requestMessage.AddVersion(request.Version);
        requestMessage.Content = stringContent;
        await AddAuthenticationHeader(requestMessage);
        requestMessage.AddCorrelationId();

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        await response.EnsureSuccessStatusCodeIncludeContentInException();
    }

    public async Task<ApiResponse<TResponse>> PutWithResponseCode<TResponse>(IPutApiRequest request) where TResponse : class
    {
        return await PutWithResponseCode<object, TResponse>(request);
    }

    public async Task<ApiResponse<TResponse>> PutWithResponseCode<TData, TResponse>(IPutApiRequest<TData> request)
    {
        var stringContent = request.Data != null ? new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json") : null;

        var requestMessage = new HttpRequestMessage(HttpMethod.Put, request.PutUrl);
        requestMessage.AddVersion(request.Version);
        requestMessage.Content = stringContent;
        await AddAuthenticationHeader(requestMessage);
        requestMessage.AddCorrelationId();

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var errorContent = "";
        var responseBody = (TResponse)default;

        if (IsNot200RangeResponseCode(response.StatusCode))
        {
            errorContent = json;
            HandleException(response, json);
        }
        else if (typeof(TResponse) != typeof(NullResponse))
        {
            responseBody = JsonSerializer.Deserialize<TResponse>(json, JsonSerializationOptions);
        }

        var apiResponse = new ApiResponse<TResponse>(responseBody, response.StatusCode, errorContent);

        return apiResponse;
    }

    public async Task<IEnumerable<TResponse>> GetAll<TResponse>(IGetAllApiRequest request)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetAllUrl);
        requestMessage.AddVersion(request.Version);
        await AddAuthenticationHeader(requestMessage);
        requestMessage.AddCorrelationId();

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            return new List<TResponse>();
        }

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<IEnumerable<TResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<HttpStatusCode> GetResponseCode(IGetApiRequest request)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);
        requestMessage.AddVersion(request.Version);
        await AddAuthenticationHeader(requestMessage);
        requestMessage.AddCorrelationId();

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);

        return response.StatusCode;
    }

    public async Task<PagedResponse<TResponse>> GetPaged<TResponse>(IGetPagedApiRequest request)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetPagedUrl);
        requestMessage.AddVersion(request.Version);
        await AddAuthenticationHeader(requestMessage);

        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            return new PagedResponse<TResponse>{Data = []};
        }

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<PagedResponse<TResponse>>(json);
    }
    private static bool IsNot200RangeResponseCode(HttpStatusCode statusCode)
    {
        return !((int)statusCode >= 200 && (int)statusCode <= 299);
    }
}
