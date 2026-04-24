namespace SFA.DAS.ApiContracts.Build.Tests.ApiResponses;

using V2Responses = SFA.DAS.ApiContracts.Build.Tests.V2.ApiResponses;

public class WhenGeneratingResponseDtos
{
    [Test]
    public void DasRequestResponse_CreatedAt_IsDateTime_NotDateTimeOffset()
    {
        typeof(DasRequestResponse)
            .GetProperty(nameof(DasRequestResponse.CreatedAt))!
            .PropertyType
            .Should().Be(typeof(DateTime));
    }

    [Test]
    public void V2_DasRequestResponse_CreatedAt_IsDateTime_NotDateTimeOffset()
    {
        typeof(V2Responses.DasRequestResponse)
            .GetProperty(nameof(V2Responses.DasRequestResponse.CreatedAt))!
            .PropertyType
            .Should().Be(typeof(DateTime));
    }
}
