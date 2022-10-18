using System.Security.Principal;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Data;
using SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests.Data;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests;
public class WhenShorteningRuleNames
{
    [Test, AutoData]
    public void And_the_name_is_over_50_Then_name_eventname_should_be_shorthened()
    {
        var type = typeof(TestEvent);
        var name = AzureRuleNameShortener.Shorten(type);
        name.Should().Contain("TestEvent");
        name.Should().NotBeEquivalentTo("NServiceBus.AzureFunction.Extensions.UnitTests.Data.TestEvent");
        name.Length.Should().BeLessThanOrEqualTo(50);
    }

    [Test, AutoData]
    public void And_the_name_is_not_over_50_Then_name_eventname_should_stay_the_same()
    {
        var type = typeof(ShortNameEvent);
        var name = AzureRuleNameShortener.Shorten(type);
        name.Should().Be("Data.ShortNameEvent");
    }
}