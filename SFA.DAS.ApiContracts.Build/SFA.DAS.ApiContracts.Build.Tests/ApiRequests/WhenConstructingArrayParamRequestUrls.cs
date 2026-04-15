using SFA.DAS.ApiContracts.Build.Tests.ApiResponses;

namespace SFA.DAS.ApiContracts.Build.Tests.ApiRequests;

public class WhenConstructingArrayParamRequestUrls
{
    private static readonly System.Guid Id = System.Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Test]
    public void ArrayParam_IsGeneratedAs_ListOfItemType()
    {
        var request = new GetDasRequestsByDasRequestIdItemsApiRequest(Id, null, null, null);
        request.Status.Should().BeNull();
    }

    [Test]
    public void ArrayParam_NullList_OmitsParamFromUrl()
    {
        var request = new GetDasRequestsByDasRequestIdItemsApiRequest(Id, null, null, null);
        request.GetUrl.Should().NotContain("status").And.NotContain("tags");
    }

    [Test]
    public void ArrayParam_SingleEnumValue_ProducesSingleRepeatedParam()
    {
        var request = new GetDasRequestsByDasRequestIdItemsApiRequest(
            Id,
            [DasRequestSortOrder.Name],
            null,
            null);

        Uri.UnescapeDataString(request.GetUrl).Should().Contain("status=Name");
    }

    [Test]
    public void ArrayParam_MultipleEnumValues_ProducesRepeatedParams()
    {
        var request = new GetDasRequestsByDasRequestIdItemsApiRequest(
            Id,
            [DasRequestSortOrder.Name, DasRequestSortOrder.Created],
            null,
            null);

        var url = Uri.UnescapeDataString(request.GetUrl);
        url.Should().Contain("status=Name").And.Contain("status=Created");
    }

    [Test]
    public void ArrayParam_StringList_ProducesRepeatedParams()
    {
        var request = new GetDasRequestsByDasRequestIdItemsApiRequest(
            Id,
            null,
            ["alpha", "beta"],
            null);

        var url = Uri.UnescapeDataString(request.GetUrl);
        url.Should().Contain("tags=alpha").And.Contain("tags=beta");
    }

    [Test]
    public void ArrayParam_MixedWithScalar_AllParamsPresent()
    {
        var request = new GetDasRequestsByDasRequestIdItemsApiRequest(
            Id,
            [DasRequestSortOrder.Name],
            ["alpha"],
            3);

        var url = Uri.UnescapeDataString(request.GetUrl);
        url.Should().Contain("status=Name")
            .And.Contain("tags=alpha")
            .And.Contain("page=3");
    }

    [Test]
    public void ArrayParam_NullScalar_OmitsScalarFromUrl()
    {
        var request = new GetDasRequestsByDasRequestIdItemsApiRequest(
            Id,
            [DasRequestSortOrder.Name],
            null,
            null);

        var url = Uri.UnescapeDataString(request.GetUrl);
        url.Should().Contain("status=Name")
            .And.NotContain("tags")
            .And.NotContain("page");
    }
}
