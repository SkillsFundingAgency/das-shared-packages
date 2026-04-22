namespace SFA.DAS.ApiContracts.Build.Tests.ApiRequests;

using V2Responses = SFA.DAS.ApiContracts.Build.Tests.V2.ApiResponses;

public class WhenConstructingVersionedRequests
{
    [Test]
    public void V1_GetRequest_HasVersion1()
    {
        IBaseApiRequest request = new GetDasRequestsByDasRequestIdApiRequest(Guid.NewGuid());
        request.Version.Should().Be("1.0");
    }

    [Test]
    public void V1_PostRequest_HasVersion1()
    {
        IBaseApiRequest request = new PostDasRequestsApiRequest(new PostDasRequest());
        request.Version.Should().Be("1.0");
    }

    [Test]
    public void V1_PutRequest_HasVersion1()
    {
        IBaseApiRequest request = new PutDasRequestsByDasRequestIdApiRequest { DasRequestId = Guid.NewGuid() };
        request.Version.Should().Be("1.0");
    }

    [Test]
    public void V1_DeleteRequest_HasVersion1()
    {
        IBaseApiRequest request = new DeleteDasRequestsByDasRequestIdApiRequest(Guid.NewGuid());
        request.Version.Should().Be("1.0");
    }

    [Test]
    public void V1_PatchRequest_HasVersion1()
    {
        IBaseApiRequest request = new PatchDasRequestsByDasRequestIdApiRequest { DasRequestId = Guid.NewGuid() };
        request.Version.Should().Be("1.0");
    }

    [Test]
    public void V2_GetRequest_HasVersion2()
    {
        var request = new V2.GetDasRequestsByDasRequestIdApiRequest(Guid.NewGuid());
        request.Version.Should().Be("2.0");
    }

    [Test]
    public void V2_PostRequest_HasVersion2()
    {
        var request = new V2.PostDasRequestsApiRequest(new V2Responses.PostDasRequest());
        request.Version.Should().Be("2.0");
    }

    [Test]
    public void V2_PutRequest_HasVersion2()
    {
        var request = new V2.PutDasRequestsByDasRequestIdApiRequest { DasRequestId = Guid.NewGuid() };
        request.Version.Should().Be("2.0");
    }

    [Test]
    public void V2_DeleteRequest_HasVersion2()
    {
        var request = new V2.DeleteDasRequestsByDasRequestIdApiRequest(Guid.NewGuid());
        request.Version.Should().Be("2.0");
    }

    [Test]
    public void V2_PatchRequest_HasVersion2()
    {
        var request = new V2.PatchDasRequestsByDasRequestIdApiRequest { DasRequestId = Guid.NewGuid() };
        request.Version.Should().Be("2.0");
    }

    [Test]
    public void V2_GetRequest_ImplementsIGetApiRequest()
    {
        var request = new V2.GetDasRequestsByDasRequestIdApiRequest(Guid.NewGuid());
        request.Should().BeAssignableTo<IGetApiRequest>();
    }

    [Test]
    public void V2_PostRequest_ImplementsIPostApiRequest()
    {
        var request = new V2.PostDasRequestsApiRequest(new V2Responses.PostDasRequest());
        request.Should().BeAssignableTo<IPostApiRequest>();
    }

    [Test]
    public void V2_GetRequest_UrlIsUnchanged()
    {
        var dasRequestId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var request = new V2.GetDasRequestsByDasRequestIdApiRequest(dasRequestId);
        request.GetUrl.Should().Be($"api/das-requests/{dasRequestId}");
    }

    [Test]
    public void V2_PostRequest_UrlIsUnchanged()
    {
        var request = new V2.PostDasRequestsApiRequest(new V2Responses.PostDasRequest());
        request.PostUrl.Should().Be("api/das-requests");
    }
}
