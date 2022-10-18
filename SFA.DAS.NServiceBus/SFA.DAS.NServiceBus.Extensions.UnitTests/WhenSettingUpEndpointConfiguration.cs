using NServiceBus;
using NUnit.Framework;


namespace SFA.DAS.NServiceBus.Extensions.UnitTests;

public class WhenSettingUpEndpointConfiguration
{
    [Test]
    public void Then_UseSerializer_does_not_throw_error()
    {
        var sut = new EndpointConfiguration("test");
        sut.UseNewtonsoftJsonSerializer();
    }

    [Test]
    public void Then_UseMessageConventions_does_not_throw_error()
    {
        var sut = new EndpointConfiguration("test");
        sut.UseMessageConventions();
    }
}
