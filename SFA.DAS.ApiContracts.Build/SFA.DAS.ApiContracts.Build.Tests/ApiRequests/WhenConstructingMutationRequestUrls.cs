namespace SFA.DAS.ApiContracts.Build.Tests.ApiRequests;

public class WhenConstructingMutationRequestUrls
{
    [Test]
    public void PutDasRequestsByDasRequestId_ImplementsIPutApiRequest()
    {
        var request = new PutDasRequestsByDasRequestIdApiRequest { DasRequestId = Guid.NewGuid() };
        request.Should().BeAssignableTo<IPutApiRequest<PutDasRequest>>();
    }

    [Test]
    public void PutDasRequestsByDasRequestId_UrlInterpolatesDasRequestId()
    {
        var dasRequestId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var request = new PutDasRequestsByDasRequestIdApiRequest { DasRequestId = dasRequestId };
        request.PutUrl.Should().Be($"api/das-requests/{dasRequestId}");
    }

    [Test]
    public void PutDasRequestsByDasRequestId_DataPropertyIsCorrectType()
    {
        var request = new PutDasRequestsByDasRequestIdApiRequest
        {
            DasRequestId = Guid.NewGuid(),
            Data = new PutDasRequest { Name = "Test" }
        };
        request.Data.Should().BeOfType<PutDasRequest>();
    }

    [Test]
    public void DeleteDasRequestsByDasRequestId_ImplementsIDeleteApiRequest()
    {
        var dasRequestId = Guid.NewGuid();
        var request = new DeleteDasRequestsByDasRequestIdApiRequest(dasRequestId);
        request.Should().BeAssignableTo<IDeleteApiRequest>();
    }

    [Test]
    public void DeleteDasRequestsByDasRequestId_UrlInterpolatesDasRequestId()
    {
        var dasRequestId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var request = new DeleteDasRequestsByDasRequestIdApiRequest(dasRequestId);
        request.DeleteUrl.Should().Be($"api/das-requests/{dasRequestId}");
    }

    [Test]
    public void PatchDasRequestsByDasRequestId_ImplementsIPatchApiRequest()
    {
        var request = new PatchDasRequestsByDasRequestIdApiRequest { DasRequestId = Guid.NewGuid() };
        request.Should().BeAssignableTo<IPatchApiRequest<List<string>>>();
    }

    [Test]
    public void PatchDasRequestsByDasRequestId_UrlInterpolatesDasRequestId()
    {
        var dasRequestId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var request = new PatchDasRequestsByDasRequestIdApiRequest { DasRequestId = dasRequestId };
        request.PatchUrl.Should().Be($"api/das-requests/{dasRequestId}");
    }

    [Test]
    public void PatchDasRequestsByDasRequestId_InlineArrayBodyTypeMappedToListOfString()
    {
        var request = new PatchDasRequestsByDasRequestIdApiRequest
        {
            DasRequestId = Guid.NewGuid(),
            Data = ["op1", "op2"]
        };
        request.Data.Should().BeOfType<List<string>>();
    }

    [Test]
    public void PostDasRequests_ImplementsIPostApiRequest()
    {
        var request = new PostDasRequestsApiRequest();
        request.Should().BeAssignableTo<IPostApiRequest>();
    }

    [Test]
    public void PostDasRequests_UrlIsBaseResourcePath()
    {
        var request = new PostDasRequestsApiRequest();
        request.PostUrl.Should().Be("api/das-requests");
    }

    [Test]
    public void PostDasRequests_RequestDataPropertyIsStronglyTyped()
    {
        var request = new PostDasRequestsApiRequest { RequestData = new PostDasRequest { Name = "DasRequest A" } };
        request.RequestData.Should().BeOfType<PostDasRequest>();
        request.RequestData.Name.Should().Be("DasRequest A");
    }

    [Test]
    public void PostDasRequestsBatch_ImplementsIPostApiRequestOfListGuid()
    {
        var request = new PostDasRequestsBatchApiRequest();
        request.Should().BeAssignableTo<IPostApiRequest<List<Guid>>>();
    }

    [Test]
    public void PostDasRequestsBatch_UrlPointsToBatchEndpoint()
    {
        var request = new PostDasRequestsBatchApiRequest();
        request.PostUrl.Should().Be("api/das-requests/batch");
    }

    [Test]
    public void PostDasRequestsBatch_InlineArrayOfUuidMappedToListOfGuid()
    {
        var ids = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };
        var request = new PostDasRequestsBatchApiRequest { Data = ids };
        request.Data.Should().BeEquivalentTo(ids);
    }

    [Test]
    public void PutDasRequestsByDasRequestId_IsClassNotRecord()
    {
        typeof(PutDasRequestsByDasRequestIdApiRequest).IsClass.Should().BeTrue();
        typeof(PutDasRequestsByDasRequestIdApiRequest).GetMethod("<Clone>$").Should().BeNull("records have a Clone method; classes do not");
    }

    [Test]
    public void DeleteDasRequestsByDasRequestId_IsRecord()
    {
        typeof(DeleteDasRequestsByDasRequestIdApiRequest).GetMethod("<Clone>$").Should().NotBeNull("records expose a compiler-generated Clone method");
    }
}
