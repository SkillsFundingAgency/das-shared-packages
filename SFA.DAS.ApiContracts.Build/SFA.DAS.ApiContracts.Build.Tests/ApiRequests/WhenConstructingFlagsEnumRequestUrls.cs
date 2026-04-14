using SFA.DAS.ApiContracts.Build.Tests.ApiResponses;

namespace SFA.DAS.ApiContracts.Build.Tests.ApiRequests;

public class WhenConstructingFlagsEnumRequestUrls
{
    [Test]
    public void DasRequestRuleSet_IsDecoratedWithFlagsAttribute()
    {
        typeof(DasRequestRuleSet).Should().BeDecoratedWith<FlagsAttribute>();
    }

    [Test]
    public void DasRequestRuleSet_HasLongBackingType()
    {
        Enum.GetUnderlyingType(typeof(DasRequestRuleSet)).Should().Be(typeof(long));
    }

    [Test]
    public void DasRequestRuleSet_ValuesArePowersOfTwo()
    {
        DasRequestRuleSet.None.Should().Be((DasRequestRuleSet)0);
        DasRequestRuleSet.Name.Should().Be((DasRequestRuleSet)1);
        DasRequestRuleSet.Description.Should().Be((DasRequestRuleSet)2);
        DasRequestRuleSet.Status.Should().Be((DasRequestRuleSet)4);
    }

    [Test]
    public void ValidateDasRequestsByDasRequestId_SingleFlag_ProducesNameInUrl()
    {
        var id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var request = new PutDasRequestsByDasRequestIdValidateApiRequest
        {
            DasRequestId = id,
            RuleSet = DasRequestRuleSet.Name
        };

        request.PutUrl.Should().Contain("ruleSet=Name");
    }

    [Test]
    public void ValidateDasRequestsByDasRequestId_CombinedFlags_ProducesCommaJoinedNamesWithNoSpaces()
    {
        var id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var request = new PutDasRequestsByDasRequestIdValidateApiRequest
        {
            DasRequestId = id,
            RuleSet = DasRequestRuleSet.Name | DasRequestRuleSet.Description
        };

        Uri.UnescapeDataString(request.PutUrl).Should().Contain("ruleSet=Name,Description")
            .And.NotContain(" ");
    }

    [Test]
    public void ValidateDasRequestsByDasRequestId_NullRuleSet_OmitsParamFromUrl()
    {
        var id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var request = new PutDasRequestsByDasRequestIdValidateApiRequest
        {
            DasRequestId = id,
            RuleSet = null
        };

        request.PutUrl.Should().NotContain("ruleSet");
    }
}
