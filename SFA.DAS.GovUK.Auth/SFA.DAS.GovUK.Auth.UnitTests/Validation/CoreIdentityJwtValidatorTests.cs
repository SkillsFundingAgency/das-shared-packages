using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Exceptions;
using SFA.DAS.GovUK.Auth.Helper;
using SFA.DAS.GovUK.Auth.Validation;

namespace SFA.DAS.GovUK.Auth.UnitTests.Validation;

[TestFixture]
public class CoreIdentityJwtValidatorTests
{
    private CoreIdentityJwtValidator _sut;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private HttpClient _httpClient;
    private GovUkOidcConfiguration _config;
    private Mock<ILogger<CoreIdentityJwtValidator>> _loggerMock;
    private Mock<IDateTimeHelper> _dateTimeHelperMock;
    private DateTimeOffset _now;

    [SetUp]
    public void Setup()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _config = new GovUkOidcConfiguration { BaseUrl = "https://identity.account.gov.uk" };
        _loggerMock = new Mock<ILogger<CoreIdentityJwtValidator>>();
        _dateTimeHelperMock = new Mock<IDateTimeHelper>();

        _now = new DateTimeOffset(2025, 6, 23, 12, 0, 0, TimeSpan.Zero);
        _dateTimeHelperMock.Setup(d => d.UtcNowOffset).Returns(() => _now);

        _sut = new CoreIdentityJwtValidator(_httpClient, _config, _dateTimeHelperMock.Object, _loggerMock.Object);
    }

    [TearDown]
    public void Teardown()
    {
        if(_httpClient != null ) 
            _httpClient.Dispose();
        
        if(_sut != null)
            _sut.Dispose();
    }

    private static string ValidDidJson => @"{
        ""id"": ""controller123"",
        ""assertionMethod"": [
            {
                ""id"": ""controller123#key1"",
                ""controller"": ""controller123"",
                ""type"": ""JsonWebKey"",
                ""publicKeyJwk"": {
                    ""kty"": ""RSA"",
                    ""e"": ""AQAB"",
                    ""n"": ""testmodulus""
                }
            }
        ]
    }";

    private static string InvalidDidJsonMissingId => @"{
        ""assertionMethod"": []
    }";

    private static string InvalidDidJsonMissingAssertionMethod => @"{
        ""id"": ""controller123""
    }";

    private void SetupDidResponse(string json, TimeSpan? maxAge = null)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };

        if (maxAge != null)
        {
            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = maxAge
            };
        }

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    [Test]
    public async Task EnsureDidDocument_Loads_DID_When_NotCached()
    {
        SetupDidResponse(ValidDidJson, TimeSpan.FromMinutes(30));

        await _sut.LoadDidDocument();

        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync", Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Test]
    public async Task EnsureDidDocument_Uses_Cached_DID_When_NotExpired()
    {
        SetupDidResponse(ValidDidJson, TimeSpan.FromMinutes(30));

        await _sut.LoadDidDocument();
        await _sut.LoadDidDocument();

        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync", Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Test]
    public async Task EnsureDidDocument_Refreshes_DID_When_Expired()
    {
        SetupDidResponse(ValidDidJson, TimeSpan.FromMinutes(30));
        await _sut.LoadDidDocument();

        _now = _now.AddMinutes(31);
        SetupDidResponse(ValidDidJson, TimeSpan.FromMinutes(60));

        await _sut.LoadDidDocument();

        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync", Times.Exactly(2),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Test]
    public void ValidateCoreIdentity_Throws_IfDidNotLoaded()
    {
        Action act = () => _sut.ValidateCoreIdentity("fake-jwt");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("DID has not been fetched.");
    }

    [Test]
    public async Task LoadDid_Throws_When_Id_Missing()
    {
        SetupDidResponse(InvalidDidJsonMissingId);

        Func<Task> act = async () => await _sut.LoadDidDocument();

        await act.Should().ThrowAsync<DidLoadException>()
            .WithMessage("DID does not contain an 'id' string property.");
    }

    [Test]
    public async Task LoadDid_Throws_When_AssertionMethod_Missing()
    {
        SetupDidResponse(InvalidDidJsonMissingAssertionMethod);

        Func<Task> act = async () => await _sut.LoadDidDocument();

        await act.Should().ThrowAsync<DidLoadException>()
            .WithMessage("DID does not contain an 'assertionMethod' array property.");
    }
}
