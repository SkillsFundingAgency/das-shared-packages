using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using static SFA.DAS.Testing.AutoFixture.UnitTests.FixtureBuilderExtensions.WithValuesTests;

namespace SFA.DAS.Testing.AutoFixture.UnitTests.FixtureBuilderExtensions;

public class CreateManyTests
{
    [Test]
    public void CreatesManyFromGivenValues()
    {
        Fixture fixture = new();
        int[] ids = [1, 5, 9];

        var people = fixture.Build<Person>().CreateMany(p => p.Id, ids);

        people.Any(p => p.Id == 1).Should().BeTrue();
        people.Any(p => p.Id == 5).Should().BeTrue();
        people.Any(p => p.Id == 9).Should().BeTrue();

        people.Should().AllSatisfy(p => p.Id.Should().BeOneOf(ids));
    }

    [Test]
    public void ChainsWithValuesAndCreateMany()
    {
        Fixture fixture = new();
        int[] ids = [1, 5, 9];
        string[] names = ["Tom", "Dick", "Harry"];

        var people = fixture.Build<Person>().WithValues(p => p.Name, names).CreateMany(p => p.Id, ids);

        people.Select(p => p.Id).Should().Equal(ids);
        people.Select(p => p.Name).Should().Equal(names);
    }
}

