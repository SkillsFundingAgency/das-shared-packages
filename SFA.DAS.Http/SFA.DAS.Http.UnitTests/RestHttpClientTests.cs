using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Http.UnitTests.Fakes;
using SFA.DAS.Testing;


namespace SFA.DAS.Http.UnitTests.REST
{
    [TestFixture]
    [Parallelizable]
    public class RestHttpClientTests : FluentTest<RestClientTestsFixture>
    {
        [Test]
        public Task WhenCallingGetAndHttpClientReturnsSuccess_ThenShouldReturnResponseBodyAsString()
        {
            return RunAsync(f => f.SetupHttpClientToReturnSuccessWithStringResponseBody(), f => f.CallGet(null), 
                (f, r) =>
                {
                    r.Should().NotBeNull();
                    r.Should().Be(f.ResponseString);
                });
        }
        
        [Test]
        public Task WhenCallingGenericGetAndHttpClientReturnsSuccess_ThenShouldReturnResponseBodyAsObject()
        {
            return RunAsync(f => f.SetupHttpClientToReturnSuccessWithJsonObjectInResponseBody(), f => f.CallGetObject(null), 
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
            return RunAsync(f => f.SetupHttpClientToReturnSuccessWithStringResponseBody(),
                f => f.CallGet(new {QueryParam1 = "qp1", QueryParam2 = "qp2"}),
                (f, r) => f.HttpMessageHandler.HttpRequestMessage.RequestUri.Should().Be($"{f.HttpClient.BaseAddress}?QueryParam1=qp1&QueryParam2=qp2"));
        }

        [Test]
        public Task WhenCallingPostAsJsonExpectingStringAndHttpClientReturnsSuccess_ThenShouldReturnResponseBodyAsString()
        {
            const string expectedResponse = "Hello World!";

            return RunAsync(
                f => f.SetupHttpClientToReturnSuccessWithStringResponseBody(expectedResponse), 
                f => f.CallPostAsJson(new {Property1 = "Hello who?"}),
                (f, r) => r.Should().Be(expectedResponse));
        }

        [Test]
        public Task WhenCallingPostAsJsonExpectingObjectAndHttpClientReturnsSuccess_ThenShouldReturnResponseBodyAsObject()
        {
            var expectedResponse = new TestObjectToUseAsAResponse
            {
                BoolField = true,
                DateTimeField = DateTime.Now,
                IntField = new Random().Next(),
                StringField = Guid.NewGuid().ToString()
            };

            return RunAsync(
                f => f.SetupHttpClientToReturnSuccessWithJsonObjectInResponseBody(expectedResponse),
                f => f.CallPostAsJson<TestObjectToUseAsARequest, TestObjectToUseAsAResponse>(new TestObjectToUseAsARequest {StringField = "Hello who?"}),
                (f, r) => r.Should().BeEquivalentTo(expectedResponse));
        }


        [Test]
        public Task WhenCallingPostAsJsonAndHttpClientReturnsNonSuccess_ThenShouldThrowRestClientException()
        {
            return RunAsync(f => f.SetupHttpClientPostToReturnInternalServerErrorWithStringResponseBody(), f => f.CallGet(null),
                (f, r) => r.Should().Throw<RestHttpClientException>()
                    .Where(ex => ex.StatusCode == HttpStatusCode.InternalServerError
                                 && ex.ReasonPhrase == "Internal Server Error"
                                 && Equals(ex.RequestUri, f.RequestUri)
                                 && ex.ErrorResponse.Contains(f.ResponseString)));
        }
    }

    public class TestObjectToUseAsARequest
    {
        public string StringField { get; set; }
    }

    public class TestObjectToUseAsAResponse
    {
        public string StringField { get; set; }
        public int IntField { get; set; }
        public DateTime DateTimeField { get; set; }
        public bool BoolField { get; set; }
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
        
        public void SetupHttpClientToReturnSuccessWithStringResponseBody(string responseString = "Test")
        {
            ResponseString = responseString;
            HttpMessageHandler.HttpResponseMessage = new HttpResponseMessage { Content = new StringContent(ResponseString, Encoding.Default, "text/plain") };
        }

        public void SetupHttpClientToReturnSuccessWithJsonObjectInResponseBody()
        {
            SetupHttpClientToReturnSuccessWithJsonObjectInResponseBody(new ExampleResponseObject { StringProperty = "Test property" });
        }

        public void SetupHttpClientToReturnSuccessWithJsonObjectInResponseBody<TResponse>(TResponse responseData)
        {
            var stringBody = JsonConvert.SerializeObject(responseData);
            ResponseObject = responseData;
            HttpMessageHandler.HttpResponseMessage = new HttpResponseMessage { Content = new StringContent(stringBody, Encoding.Default, "application/json") };
        }

        public void SetupHttpClientGetToReturnInternalServerErrorWithStringResponseBody()
        {
            SetupHttpClientToReturnInternalServerErrorWithStringResponseBody(HttpMethod.Get);
        }

        public void SetupHttpClientPostToReturnInternalServerErrorWithStringResponseBody()
        {
            SetupHttpClientToReturnInternalServerErrorWithStringResponseBody(HttpMethod.Post);
        }

        public void SetupHttpClientToReturnInternalServerErrorWithStringResponseBody(HttpMethod httpMethod)
        {
            ResponseString = "Some sort of error description";
            RequestUri = new Uri($"{HttpClient.BaseAddress}/request", UriKind.Absolute);
            HttpMessageHandler.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ResponseString, Encoding.Default, "text/plain"),
                RequestMessage = new HttpRequestMessage(httpMethod, RequestUri)
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

        public Task<string> CallPostAsJson<TRequestData>(TRequestData requestData)
        {
            return RestHttpClient.PostAsJson<TRequestData>("https://example.com", requestData);
        }

        public Task<TResponseData> CallPostAsJson<TRequestData, TResponseData>(TRequestData requestData)
        {
            return RestHttpClient.PostAsJson<TRequestData, TResponseData>("https://example.com", requestData);
        }
    }
}