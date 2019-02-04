using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Http.REST;
using SFA.DAS.Http.UnitTests.Fakes;
using SFA.DAS.Testing;


namespace SFA.DAS.Http.UnitTests.REST
{
    [TestFixture]
    [Parallelizable]
    public class RestClientTests : FluentTest<RestClientTestsFixture>
    {
        [Test]
        public Task WhenCallingGetAndHttpClientReturnsSuccess_ThenShouldReturnResponseBodyAsString()
        {
            return RunAsync(f => f.SetupHttpClientGetToReturnSuccessWithStringResponseBody(), f => f.CallGet(null), 
                (f, r) =>
                {
                    r.Should().NotBeNull();
                    r.Should().Be(f.ResponseString);
                });
        }
        
        [Test]
        public Task WhenCallingGenericGetAndHttpClientReturnsSuccess_ThenShouldReturnResponseBodyAsObject()
        {
            return RunAsync(f => f.SetupHttpClientGetToReturnSuccessWithJsonObjectInResponseBody(), f => f.CallGetObject(null), 
                (f, r) =>
                {
                    r.Should().NotBeNull();
                    r.Should().BeEquivalentTo(f.ResponseObject);
                });
        }
        
        [Test]
        public Task WhenCallingGetAndHttpClientReturnsNonSuccess_ThenShouldThrowRestClientException()
        {
            return RunAsync(f => f.SetupHttpClientGetToReturnInternalServerErrorWithStringResponseBody(), f => f.CallGet(null),
                (f, r) => r.Should().Throw<RestHttpClientException>()
                    .Where(ex => ex.StatusCode == HttpStatusCode.InternalServerError
                                 && ex.ReasonPhrase == "Internal Server Error"
                                 && Equals(ex.RequestUri, f.RequestUri)
                                 && ex.ErrorResponse.Contains(f.ResponseString)));
        }

        [Test]
        public Task WhenCallingGetAndQueryDataIsSupplied_ThenUriCalledShouldContainTheQueryData()
        {
            return RunAsync(f => f.SetupHttpClientGetToReturnSuccessWithStringResponseBody(),
                f => f.CallGet(new {QueryParam1 = "qp1", QueryParam2 = "qp2"}),
                (f, r) => f.HttpMessageHandler.HttpRequestMessage.RequestUri.Should().Be($"{f.HttpClient.BaseAddress}?QueryParam1=qp1&QueryParam2=qp2"));
        }
    }

    public class RestClientTestsFixture
    {
        public class ExampleResponseObject
        {
            public string StringProperty { get; set; }
        }
        
        public FakeHttpMessageHandler HttpMessageHandler { get; set; }
        public HttpClient HttpClient { get; set; }
        public IRestHttpClient RestHttpClient { get; set; }
        public Uri RequestUri { get; set; }
        public string ResponseString { get; set; }
        public object ResponseObject { get; set; }
        
        public RestClientTestsFixture()
        {
            HttpMessageHandler = new FakeHttpMessageHandler();
            HttpClient = new HttpClient(HttpMessageHandler) { BaseAddress = new Uri("https://example.com") };
            RestHttpClient = new RestHttpClient(HttpClient);
        }
        
        public void SetupHttpClientGetToReturnSuccessWithStringResponseBody()
        {
            ResponseString = "Test";
            HttpMessageHandler.HttpResponseMessage = new HttpResponseMessage { Content = new StringContent(ResponseString, Encoding.Default, "text/plain") };
        }

        public void SetupHttpClientGetToReturnSuccessWithJsonObjectInResponseBody()
        {
            ResponseObject = new ExampleResponseObject {StringProperty = "Test property"};
                
            var stringBody = JsonConvert.SerializeObject(ResponseObject);
            HttpMessageHandler.HttpResponseMessage = new HttpResponseMessage { Content = new StringContent(stringBody, Encoding.Default, "application/json") };
        }

        public void SetupHttpClientGetToReturnInternalServerErrorWithStringResponseBody()
        {
            ResponseString = "Some sort of error description";
            RequestUri = new Uri($"{HttpClient.BaseAddress}/request", UriKind.Absolute);
            HttpMessageHandler.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ResponseString, Encoding.Default, "text/plain"),
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, RequestUri)
            };
        }

        public async Task<string> CallGet(object queryData)
        {
            return await RestHttpClient.Get("https://example.com", queryData);
        }

        public async Task<ExampleResponseObject> CallGetObject(object queryData)
        {
            return await RestHttpClient.Get<ExampleResponseObject>("https://example.com", queryData);
        }
    }
}