using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.Testing.UnitTests.Builder
{
    [TestFixture]
    [Parallelizable]
    public class ObjectExtensonsTests : FluentTest<ObjectExtensonsTestsFixture>
    {
        [Test]
        public void ObjectExtensions_WhenSettingAPrivateProperty_ThenShouldSetValueCorrectly()
        {
            Test(f => f.Activate().Set(x => x.IntProperty, 4).IntProperty.Should().Be(4));
        }

        [Test]
        public void ObjectExtensions_WhenSettingAReadOnlyProperty_ThenShouldSetValueCorrectly()
        {
            TestException(f => f.Activate().Set(x => x.ReadOnlyStringProperty, "A new String"), (f,r) => r.Should().Throw<ArgumentException>());
        }

        [Test]
        public void ObjectExtensions_WhenSettingAField_ThenShouldSetValueCorrectly()
        {
            Test(f => f.Activate().Set(x => x.StringField, "my value").StringField.Should().Be("my value"));
        }

        [Test]
        public void ObjectExtensions_WhenAddingAnItemToAFieldCollection_ThenShouldAddASingleItem()
        {
            Test(f => f.Activate().Add(x => x.ListOfObjects, "New Item").ListOfObjects.Count().Should().Be(1));
        }

        [Test]
        public void ObjectExtensions_WhenAddingARangeToAFieldCollection_ThenShouldAddAallItems()
        {
            Test(f => f.Activate().AddRange(x => x.ListOfObjects, new List<string>{"New Item", "Another" }).ListOfObjects.Count().Should().Be(2));
        }

        [Test]
        public void ObjectExtensions_WhenAddingAnItemToAPropertyCollection_ThenShouldAddASingleItem()
        {
            Test(f => f.Activate().Add(x => x.PropertyListOfObjects, "New Item").ListOfObjects.Count().Should().Be(1));
        }

        [Test]
        public void ObjectExtensions_WhenAddingARangeToAPropertyCollection_ThenShouldAddAallItems()
        {
            Test(f => f.Activate().AddRange(x => x.PropertyListOfObjects, new List<string> { "New Item", "Another" }).ListOfObjects.Count().Should().Be(2));
        }


    }

    public class ObjectExtensonsTestsFixture
    {
        public ObjectWithPrivateProperties Activate()
        {
            return ObjectActivator.CreateInstance<ObjectWithPrivateProperties>();
        }
    }
}