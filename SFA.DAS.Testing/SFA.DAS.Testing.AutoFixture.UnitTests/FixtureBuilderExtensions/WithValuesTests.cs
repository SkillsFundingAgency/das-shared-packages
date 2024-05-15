using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.Testing.AutoFixture.UnitTests.FixtureBuilderExtensions;

public class WithValuesTests
{
    [Test]
    public void CreatesManyWithDistinctValues()
    {
        Fixture fixture = new();
        int[] ids = [1, 5, 9];

        var people = fixture.Build<Person>().WithValues(p => p.Id, ids).CreateMany(ids.Length);

        people.Select(p => p.Id).Should().Equal(ids);
    }

    [Test]
    public void CreatesManyFromGivenValues()
    {
        Fixture fixture = new();
        string[] names = ["Tom", "Dick", "Harry"];

        var people = fixture.Build<Person>().WithValues(p => p.Name, names).CreateMany(4);

        people.Count().Should().Be(4);
        people.Select(p => p.Name).Should().Contain(names);
        people.Count(p => p.Name == "Tom").Should().Be(2);
    }

    [Test]
    public void CreateOneWithFromGiveValues()
    {
        Fixture fixture = new();
        string[] names = ["Tom", "Dick", "Harry"];

        var person = fixture.Build<Person>().WithValues(p => p.Name, names).Create();

        person.Name.Should().Be("Tom");
    }

    public record Person(int Id, string Name);
}
