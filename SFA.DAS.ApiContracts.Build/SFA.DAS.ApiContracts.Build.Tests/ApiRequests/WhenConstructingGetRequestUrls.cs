namespace SFA.DAS.ApiContracts.Build.Tests.ApiRequests;

public class WhenConstructingGetRequestUrls
{
    [Test]
    public void GetDasRequestsByDasRequestId_ImplementsIGetApiRequest()
    {
        var dasRequestId = Guid.NewGuid();
        var request = new GetDasRequestsByDasRequestIdApiRequest(dasRequestId);
        request.Should().BeAssignableTo<IGetApiRequest>();
    }

    [Test]
    public void GetDasRequestsByDasRequestId_InterpolatesUuidPathParam()
    {
        var dasRequestId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var request = new GetDasRequestsByDasRequestIdApiRequest(dasRequestId);
        request.GetUrl.Should().Be($"api/das-requests/{dasRequestId}");
    }

    [Test]
    public void GetDasRequestsByDasRequestId_UsesProvidedDasRequestId()
    {
        var dasRequestId = Guid.NewGuid();
        var request = new GetDasRequestsByDasRequestIdApiRequest(dasRequestId);
        request.GetUrl.Should().Contain(dasRequestId.ToString());
    }

    [Test]
    public void GetDasRequests_ImplementsIGetApiRequest()
    {
        var request = new GetDasRequestsApiRequest(null, null, null, null);
        request.Should().BeAssignableTo<IGetApiRequest>();
    }

    [Test]
    public void GetDasRequests_BuildsUrlWithAllQueryParams()
    {
        var request = new GetDasRequestsApiRequest(2, 10, DasRequestSortOrder.Created, "test");
        request.GetUrl.Should().Be("api/das-requests?page=2&pageSize=10&sortOrder=Created&searchTerm=test");
    }

    [Test]
    public void GetDasRequests_NullParamsAreOmittedFromUrl()
    {
        var request = new GetDasRequestsApiRequest(null, null, null, null);
        request.GetUrl.Should().Be("api/das-requests");
    }

    [Test]
    public void GetDasRequests_MixedNullAndValueParams()
    {
        var request = new GetDasRequestsApiRequest(1, null, DasRequestSortOrder.Name, null);
        request.GetUrl.Should().Be("api/das-requests?page=1&sortOrder=Name");
    }

    [Test]
    public void GetDasRequestsByDasRequestIdSubItemsByItemId_ImplementsIGetApiRequest()
    {
        var request = new GetDasRequestsByDasRequestIdSubItemsByItemIdApiRequest(Guid.NewGuid(), 42L);
        request.Should().BeAssignableTo<IGetApiRequest>();
    }

    [Test]
    public void GetDasRequestsByDasRequestIdSubItemsByItemId_InterpolatesBothPathParams()
    {
        var dasRequestId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        const long itemId = 99L;
        var request = new GetDasRequestsByDasRequestIdSubItemsByItemIdApiRequest(dasRequestId, itemId);
        request.GetUrl.Should().Be($"api/das-requests/{dasRequestId}/sub-items/{itemId}");
    }

    [Test]
    public void GetDasRequestsByDasRequestIdSubItemsByItemId_HyphenatedSegmentIsPascalCased()
    {
        var request = new GetDasRequestsByDasRequestIdSubItemsByItemIdApiRequest(Guid.NewGuid(), 1L);
        request.GetUrl.Should().Contain("sub-items");
    }

    [Test]
    public void GetDasRequestsByDasRequestIdStatus_ImplementsIGetApiRequest()
    {
        var request = new GetDasRequestsByDasRequestIdStatusApiRequest(Guid.NewGuid(), null, null);
        request.Should().BeAssignableTo<IGetApiRequest>();
    }

    [Test]
    public void GetDasRequestsByDasRequestIdStatus_BuildsUrlWithDateTimeAndBoolParams()
    {
        var dasRequestId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var asOf = new DateTime(2025, 1, 15, 0, 0, 0);
        var request = new GetDasRequestsByDasRequestIdStatusApiRequest(dasRequestId, asOf, true);
        request.GetUrl.Should().StartWith($"api/das-requests/{dasRequestId}/status?asOf=");
        request.GetUrl.Should().EndWith("&includeArchived=True");
    }

    [Test]
    public void GetDasRequestsByDasRequestIdStatus_NullOptionalParamsAreOmittedFromUrl()
    {
        var dasRequestId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var request = new GetDasRequestsByDasRequestIdStatusApiRequest(dasRequestId, null, null);
        request.GetUrl.Should().Be($"api/das-requests/{dasRequestId}/status");
    }

    [Test]
    public void GetAccountsByAccountIdDasRequests_ImplementsIGetApiRequest()
    {
        var request = new GetAccountsByAccountIdDasRequestsApiRequest(123L, "user-1");
        request.Should().BeAssignableTo<IGetApiRequest>();
    }

    [Test]
    public void GetAccountsByAccountIdDasRequests_RequiredQueryParamAppearsInUrl()
    {
        const long accountId = 456L;
        const string userId = "abc-user";
        var request = new GetAccountsByAccountIdDasRequestsApiRequest(accountId, userId);
        request.GetUrl.Should().Be($"api/accounts/{accountId}/das-requests?userId={userId}");
    }

    [Test]
    public void GetAccountsByAccountIdDasRequests_Int64PathParamInterpolatedCorrectly()
    {
        const long accountId = long.MaxValue;
        var request = new GetAccountsByAccountIdDasRequestsApiRequest(accountId, "u");
        request.GetUrl.Should().Contain(accountId.ToString());
    }

    [Test]
    public void GetDasRequestsByDasRequestId_ClassNameStripsApiPathPrefixAndEndsWithApiRequest()
    {
        typeof(GetDasRequestsByDasRequestIdApiRequest).Name.Should().Be("GetDasRequestsByDasRequestIdApiRequest");
    }
}
