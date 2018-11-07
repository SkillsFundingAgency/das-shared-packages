using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.Testing.UnitTests
{
    [TestFixture]
    [Parallelizable]
    public class FluentTestTests : FluentTest<FluentTestTestsFixture>
    {
        [Test]
        public void Test_WhenAssertingWithFixture_ThenShouldPassFixture()
        {
            Test(f => f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>());
        }
        
        [Test]
        public void Test_WhenAssertingWithFixtureAndResult_ThenShouldPassFixtureAndNullResult()
        {
            Test((f, r) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>();
                r.Should().BeNull();
            });
        }
        
        [Test]
        public void Test_WhenActingAndAssertingWithFixture_ThenShouldActAndPassFixture()
        {
            Test(f => f.Act(), f => f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>());
        }
        
        [Test]
        public void Test_WhenActingAndAssertingWithFixtureAndResult_ThenShouldActAndPassFixtureAndResult()
        {
            Test(f => f.Act(), (f, r) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>();
                r.Should().BeTrue();
            });
        }
        
        [Test]
        public void Test_WhenArrangingAndActingAndAssertingWithFixture_ThenShouldArrangeAndActAndPassFixture()
        {
            Test(f => f.Arrange(), f => f.Act(), f => f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>().Which.Arranged.Should().BeTrue());
        }
        
        [Test]
        public void Test_WhenArrangingAndActingAndAssertingWithFixtureAndResult_ThenShouldArrangeAndActAndPassFixtureAndResult()
        {
            Test(f => f.Arrange(), f => f.Act(), (f, r) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>().Which.Arranged.Should().BeTrue();
                r.Should().BeTrue();
            });
        }
        
        [Test]
        public Task TestAsync_WhenAssertingWithFixture_ThenShouldPassFixture()
        {
            return TestAsync(f => f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>());
        }
        
        [Test]
        public Task TestAsync_WhenAssertingWithFixtureAndResult_ThenShouldPassFixtureAndNullResult()
        {
            return TestAsync((f, r) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>();
                r.Should().BeNull();
            });
        }
        
        [Test]
        public Task TestAsync_WhenActingAndAssertingWithFixture_ThenShouldActAndPassFixture()
        {
            return TestAsync(f => f.ActAsync(), f => f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>());
        }
        
        [Test]
        public Task TestAsync_WhenActingAndAssertingWithFixtureAndResult_ThenShouldActAndPassFixtureAndResult()
        {
            return TestAsync(f => f.ActAsync(), (f, r) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>();
                r.Should().BeTrue();
            });
        }
        
        [Test]
        public Task TestAsync_WhenArrangingAndActingAndAssertingWithFixture_ThenShouldArrangeAndActAndPassFixture()
        {
            return TestAsync(f => f.Arrange(), f => f.ActAsync(), f => f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>().Which.Arranged.Should().BeTrue());
        }
        
        [Test]
        public Task TestAsync_WhenArrangingAndActingAndAssertingWithFixtureAndResult_ThenShouldArrangeAndActAndPassFixtureAndResult()
        {
            return TestAsync(f => f.Arrange(), f => f.ActAsync(), (f, r) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>().Which.Arranged.Should().BeTrue();
                r.Should().BeTrue();
            });
        }
        
        [Test]
        public void TestException_WhenAssertingWithFixtureAndActionAndActionThrows_ThenShouldPassFixtureAndAction()
        {
            TestException((f, a) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>();
                a.Should().Throw<Exception>();
            });
        }
        
        [Test]
        public void TestException_WhenActingAndAssertingWithFixtureAndActionAndActionThrows_ThenShouldPassFixtureAndAction()
        {
            TestException(f => f.Throw(), (f, a) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>();
                a.Should().Throw<Exception>().WithMessage("Foobar");
            });
        }
        
        [Test]
        public void TestException_WhenArrangingAndActingAndAssertingWithFixtureAndActionAndActionDoesNotThrow_ThenShouldArrangeAndActAndPassFixtureAndAction()
        {
            TestException(f => f.Arrange(), f => f.Act(), (f, a) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>().Which.Arranged.Should().BeTrue();
                a.Should().NotThrow();
            });
        }
        
        [Test]
        public void TestException_WhenArrangingAndActingAndAssertingWithFixtureAndActionAndActionThrows_ThenShouldArrangeAndActAndPassFixtureAndAction()
        {
            TestException(f => f.Arrange(), f => f.Throw(), (f, a) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>().Which.Arranged.Should().BeTrue();
                a.Should().Throw<Exception>().WithMessage("Foobar");
            });
        }
        
        [Test]
        public Task TestExceptionAsync_WhenAssertingWithFixtureAndActionAndActionThrows_ThenShouldPassFixtureAndAction()
        {
            return TestExceptionAsync((f, a) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>();
                a.Should().Throw<Exception>();
            });
        }
        
        [Test]
        public Task TestExceptionAsync_WhenActingAndAssertingWithFixtureAndActionAndActionThrows_ThenShouldPassFixtureAndAction()
        {
            return TestExceptionAsync(f => f.ThrowAsync(), (f, a) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>();
                a.Should().Throw<Exception>().WithMessage("Foobar");
            });
        }
        
        [Test]
        public Task TestExceptionAsync_WhenArrangingAndActingAndAssertingWithFixtureAndActionAndActionThrows_ThenShouldArrangeAndActAndPassFixtureAndAction()
        {
            return TestExceptionAsync(f => f.Arrange(), f => f.ThrowAsync(), (f, a) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>().Which.Arranged.Should().BeTrue();
                a.Should().Throw<Exception>().WithMessage("Foobar");
            });
        }
        
        [Test]
        public Task TestExceptionAsync_WhenArrangingAndActingAndAssertingWithFixtureAndActionAndActionDoesNotThrow_ThenShouldArrangeAndActAndPassFixtureAndAction()
        {
            return TestExceptionAsync(f => f.Arrange(), f => f.ActAsync(), (f, a) =>
            {
                f.Should().NotBeNull().And.BeOfType<FluentTestTestsFixture>().Which.Arranged.Should().BeTrue();
                a.Should().NotThrow();
            });
        }
    }

    public class FluentTestTestsFixture
    {
        public bool Arranged { get; set; }

        public bool Act()
        {
            return true;
        }

        public Task<bool> ActAsync()
        {
            return Task.FromResult(true);
        }

        public void Throw()
        {
            throw new Exception("Foobar");
        }

        public Task ThrowAsync()
        {
            return Task.FromException(new Exception("Foobar"));
        }

        public void Arrange()
        {
            Arranged = true;
        }
    }
}