using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace SFA.DAS.Testing.AutoFixture;

public static class FixtureBuilder
{
    public static IFixture MoqFixtureFactory()
    {
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        fixture.Customize(new ValidVacancyReferenceCustomization());
        fixture.Customize<DateOnly>(o => o.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));
        fixture.Customize<TimeOnly>(o => o.FromFactory((DateTime dt) => TimeOnly.FromDateTime(dt)));

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
