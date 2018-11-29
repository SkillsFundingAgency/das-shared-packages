using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.Testing.UnitTests.Builder
{
    [TestFixture]
    [Parallelizable]
    public class ObjectActivatorTests : FluentTest<ObjectActivatorTestsFixture>
    {
        [Test]
        public void ObjectActivator_WhenActivatingClassWithParameterlessConstructor_ThenShouldCreateTheObject()
        {
            Test(f => f.ActivateObjectWithAPrivateParameterlessConstructor().Should().NotBeNull());
        }

        [Test]
        public void ObjectActivator_WhenActivatingClassWithoutParameterlessConstructor_ThenShouldThrowException()
        {
            TestException(f => f.ActivateObjectWitNoParameterlessConstructor(),
                (f, r) => r.Should().Throw<MissingMethodException>());
        }
    }

    public class ObjectActivatorTestsFixture
    {
        public object ActivateObjectWithAPrivateParameterlessConstructor()
        {
            return ObjectActivator.CreateInstance<ObjectWithPrivateProperties>();
        }

        public object ActivateObjectWitNoParameterlessConstructor()
        {
            return ObjectActivator.CreateInstance<ObjectWithNoParameterlessConstructor>();
        }
    }
}