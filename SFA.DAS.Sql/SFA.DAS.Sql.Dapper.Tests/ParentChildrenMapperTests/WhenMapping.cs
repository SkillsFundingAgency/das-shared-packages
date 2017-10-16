using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.Sql.Dapper.Tests.ParentChildrenMapperTests
{
    [TestFixture]
    public class WhenMapping
    {
        [Test, AutomoqData]
        public void ThenANullLookupParameterThrowsAnArgumentNullException(ParentChildrenMapper<Parent, Child> sut)
        {
            Action act = () => sut.Map(null, x => x, x => new List<Child>());

            act.ShouldThrow<ArgumentNullException>();
        }

        [Test, AutomoqData]
        public void ThenIfLookupIsEmptyShouldAddParentObjectAndWithChild(ParentChildrenMapper<Parent, Child> sut)
        {
            var lookup = new Dictionary<int, Parent>();
            var parent = new Parent { Id = 2, Children = new List<Child>() };
            var child = new Child();

            var result = sut.Map(lookup, x => x.Id, x => x.Children)(parent, child);

            result.Should().Be(parent);
            result.Children.Should().Contain(child);
            lookup.Values.Should().Contain(parent);
        }

        [Test, AutomoqData]
        public void ThenIfLookupAlreadyContainsEntryWithIdOfParentShouldAddNewChildToExistingParent(
            ParentChildrenMapper<Parent, Child> sut)
        {
            var lookup = new Dictionary<int, Parent>();
            var existingEntry = new Parent { Id = 2, Children = new List<Child> { new Child() } };
            lookup.Add(existingEntry.Id, existingEntry);

            var parent = new Parent { Id = 2, Children = new List<Child>() };
            var child = new Child();

            sut.Map(lookup, x => x.Id, x => x.Children)(parent, child);

            lookup.Values.Count.Should().Be(1);
            lookup.Values.Single().Children.Count.Should().Be(2);
        }

        [Test, AutomoqData]
        public void ThenIfChildIsNullDontAddItToTheParentAsAChild(ParentChildrenMapper<Parent, Child> sut)
        {
            var lookup = new Dictionary<int, Parent>();

            var parent = new Parent { Id = 2, Children = new List<Child>() };
            Child child = null;

            sut.Map(lookup, x => x.Id, x => x.Children)(parent, child);

            lookup.Values.Single().Children.Count.Should().Be(0);
        }

        public class Parent
        {
            public int Id { get; set; }

            public IList<Child> Children { get; set; }
        }

        public class Child
        {
        }
    }
}
