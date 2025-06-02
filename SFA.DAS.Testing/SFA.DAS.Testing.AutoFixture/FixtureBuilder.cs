using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace SFA.DAS.Testing.AutoFixture;

public static class FixtureBuilder
{
    public static IFixture MoqFixtureFactory()
    {
        var fixture = new Fixture();
        fixture
            .Customize(new AutoMoqCustomization { ConfigureMembers = true });
        fixture
            .Customize(new ValidVacancyReferenceCustomization());

        return fixture;
    }

    public static IFixture RecursiveMoqFixtureFactory()
    {
        var fixture = MoqFixtureFactory();

        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        return fixture;
    }
}
