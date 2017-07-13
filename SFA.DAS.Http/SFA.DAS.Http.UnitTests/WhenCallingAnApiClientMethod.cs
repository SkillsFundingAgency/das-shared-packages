using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Http.UnitTests.MessageHandlers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Http.UnitTests
{
    [TestFixture]
    public class WhenCallingAnApiClientMethod
    {
        private FakeResponseHandler _fakeResponseHandler;
        private TestApiClient _client;

        [SetUp]
        public void Setup()
        {
            _fakeResponseHandler = new FakeResponseHandler();
            var httpClient = new HttpClient(_fakeResponseHandler);
            _client = new TestApiClient(httpClient);
        }

        [Test]
        public async Task ThenValidGetWillReturnContent()
        {
            const string testUrl = "http://test/get/200";
            const string responseContent = "Test Content";

            _fakeResponseHandler.AddFakeResponse(new Uri(testUrl), new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(responseContent) });

            var response = await _client.GetAsync(testUrl);

            response.Should().Be(responseContent);
        }

        [Test]
        public void ThenBadResponseFromGetWillThrowAnExeception()
        {
            const string testUrl = "http://test/get/404";

            _fakeResponseHandler.AddFakeResponse(new Uri(testUrl), new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = new StringContent(string.Empty) });

            Func<Task<string>> act = async () => await _client.GetAsync(testUrl);

            act.ShouldThrow<Exception>().WithMessage("Response status code does not indicate success: 404 (Not Found).");
        }

        [Test]
        public async Task ThenValidGetWithQueryStrignWillReturnContent()
        {
            const string testBaseUrl = "http://test/getwithquery/200";
            string testUrlWithQueryString = testBaseUrl + "?q=abc&v=xyz";
            const string responseContent = "Test Content";

            _fakeResponseHandler.AddFakeResponse(new Uri(testUrlWithQueryString), new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(responseContent) });

            var response = await _client.GetAsync(testBaseUrl, new { q = "abc", v = "xyz" });

            response.Should().Be(responseContent);
        }

        [Test]
        public void ThenBadResponseFromGetWithQueryStringWillThrowAnExeception()
        {
            const string testBaseUrl = "http://test/getwithquery/503";
            string testUrlWithQueryString = testBaseUrl + "?q=abc&v=xyz";
            const string responseContent = "Test Content";

            _fakeResponseHandler.AddFakeResponse(new Uri(testUrlWithQueryString), new HttpResponseMessage { StatusCode = HttpStatusCode.ServiceUnavailable, Content = new StringContent(responseContent) });

            Func<Task<string>> act = async () => await _client.GetAsync(testBaseUrl, new { q = "abc", v = "xyz" });

            act.ShouldThrow<Exception>().WithMessage("Response status code does not indicate success: 503 (Service Unavailable).");
        }

        [Test]
        public async Task ThenValidPostWillReturnContent()
        {
            const string testBaseUrl = "http://test/post/200";
            const string responseContent = "Test Content";

            _fakeResponseHandler.AddFakeResponse(new Uri(testBaseUrl), new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(responseContent) });

            var response = await _client.PostAsync(testBaseUrl, "Test Post Data");

            response.Should().Be(responseContent);
        }

        [Test]
        public void ThenBadResponseFromPostWillThrowAnExeception()
        {
            const string testBaseUrl = "http://test/post/404";
            const string responseContent = "Test Content";

            _fakeResponseHandler.AddFakeResponse(new Uri(testBaseUrl), new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = new StringContent(responseContent) });

            Func<Task<string>> act = async () => await _client.PostAsync(testBaseUrl, "Test Post Data");

            act.ShouldThrow<Exception>().WithMessage("Response status code does not indicate success: 404 (Not Found).");
        }

        [Test]
        public async Task ThenValidPutWillReturnContent()
        {
            const string testBaseUrl = "http://test/put/200";
            const string responseContent = "Test Content";

            _fakeResponseHandler.AddFakeResponse(new Uri(testBaseUrl), new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(responseContent) });

            var response = await _client.PutAsync(testBaseUrl, "Test Put Data");

            response.Should().Be(responseContent);
        }

        [Test]
        public void ThenBadResponseFromPutWillThrowAnExeception()
        {
            const string testBaseUrl = "http://test/put/404";
            const string responseContent = "Test Content";

            _fakeResponseHandler.AddFakeResponse(new Uri(testBaseUrl), new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = new StringContent(responseContent) });

            Func<Task<string>> act = async () => await _client.PutAsync(testBaseUrl, "Test Put Data");

            act.ShouldThrow<Exception>().WithMessage("Response status code does not indicate success: 404 (Not Found).");
        }

        [Test]
        public async Task ThenValidPatchWillReturnContent()
        {
            const string testBaseUrl = "http://test/patch/200";
            const string responseContent = "Test Content";

            _fakeResponseHandler.AddFakeResponse(new Uri(testBaseUrl), new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(responseContent) });

            var response = await _client.PatchAsync(testBaseUrl, "Test Patch Data");

            response.Should().Be(responseContent);
        }

        [Test]
        public void ThenBadResponseFromPatchWillThrowAnExeception()
        {
            const string testBaseUrl = "http://test/patch/404";
            const string responseContent = "Test Content";

            _fakeResponseHandler.AddFakeResponse(new Uri(testBaseUrl), new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = new StringContent(responseContent) });

            Func<Task<string>> act = async () => await _client.PatchAsync(testBaseUrl, "Test Patch Data");

            act.ShouldThrow<Exception>().WithMessage("Response status code does not indicate success: 404 (Not Found).");
        }

        [Test]
        public async Task ThenValidDeleteWillReturn()
        {
            const string testBaseUrl = "http://test/delete/200";
            const string responseContent = "Test Content";

            _fakeResponseHandler.AddFakeResponse(new Uri(testBaseUrl), new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(responseContent) });

            await _client.DeleteAsync(testBaseUrl, null);
        }

        [Test]
        public void ThenBadResponseFromDeleteWillThrowAnExeception()
        {
            const string testBaseUrl = "http://test/delete/404";
            const string responseContent = "Test Content";

            _fakeResponseHandler.AddFakeResponse(new Uri(testBaseUrl), new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = new StringContent(responseContent) });

            Func<Task> act = async () => await _client.DeleteAsync(testBaseUrl, null);

            act.ShouldThrow<Exception>().WithMessage("Response status code does not indicate success: 404 (Not Found).");
        }
    }

    internal class TestApiClient : ApiClientBase
    {
        public TestApiClient(HttpClient client) : base(client)
        {
        }

        public new async Task<string> GetAsync(string url)
        {
            return await base.GetAsync(url);
        }

        public new async Task<string> GetAsync(string url, object data)
        {
            return await base.GetAsync(url, data);
        }

        public new async Task<string> PostAsync(string url, string data)
        {
            return await base.PostAsync(url, data);
        }

        public new async Task<string> PutAsync(string url, string data)
        {
            return await base.PutAsync(url, data);
        }

        public new async Task<string> PatchAsync(string url, string data)
        {
            return await base.PatchAsync(url, data);
        }

        public new async Task DeleteAsync(string url, string data)
        {
            await base.DeleteAsync(url, data);
        }
    }
}
