using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Testing;
using System.Collections.Generic;

namespace SFA.DAS.Validation.UnitTests
{
    [TestFixture]
    public class ObjectExtensionsTests : FluentTest<ObjectExtensionsTestsFixture>
    {
        [Test]
        public void GetPath_WhenGettingPathAndItemIsNotPresent_ThenShouldReturnNullPath()
        {
            Run(f => f.GetPath(), (f, r) => r.Should().BeNull());
        }

        [Test]
        public void GetPath_WhenGettingPathAndItemIsPresentAtLevel0_ThenShouldReturnEmptyPath()
        {
            Run(f => f.SetLevel0(), f => f.GetPath(), (f, r) => r.Should().Be(""));
        }

        [Test]
        public void GetPath_WhenGettingPathAndItemIsPresentAtLevel1_ThenShouldReturnLevel1Path()
        {
            Run(f => f.SetLevel1(), f => f.GetPath(), (f, r) => r.Should().Be("Level1"));
        }

        [Test]
        public void GetPath_WhenGettingPathAndItemIsPresentAtLevel1Index1_ThenShouldReturnLevel1Index1Path()
        {
            Run(f => f.SetLevel1Index1(), f => f.GetPath(), (f, r) => r.Should().Be("Level1s[1]"));
        }

        [Test]
        public void GetPath_WhenGettingPathAndItemIsPresentAtLevel2_ThenShouldReturnLevel2Path()
        {
            Run(f => f.SetLevel2(), f => f.GetPath(), (f, r) => r.Should().Be("Level1.Level2"));
        }

        [Test]
        public void GetPath_WhenGettingPathAndItemIsPresentAtIndex1Level2_ThenShouldReturnIndex1Level2Path()
        {
            Run(f => f.SetIndex1Level2(), f => f.GetPath(), (f, r) => r.Should().Be("Level1s[1].Level2"));
        }

        [Test]
        public void GetPath_WhenGettingPathAndItemIsPresentAtLevel2Index1_ThenShouldReturnLevel2Index1Path()
        {
            Run(f => f.SetLevel2Index1(), f => f.GetPath(), (f, r) => r.Should().Be("Level1.Level2s[1]"));
        }

        [Test]
        public void GetPath_WhenGettingPathAndItemIsPresentAtLevel3_ThenShouldReturnLevel3Path()
        {
            Run(f => f.SetLevel3(), f => f.GetPath(), (f, r) => r.Should().Be("Level1.Level2.Level3"));
        }
    }

    public class ObjectExtensionsTestsFixture
    {
        public object Source { get; set; }
        public object Item { get; set; }

        public ObjectExtensionsTestsFixture()
        {
            Source = new Level0();
            Item = new object();
        }

        public string GetPath()
        {
            return Source.GetPath(Item);
        }

        public ObjectExtensionsTestsFixture SetLevel0()
        {
            Item = Source;

            return this;
        }

        public ObjectExtensionsTestsFixture SetLevel1()
        {
            Item = new Level1();

            Source = new Level0
            {
                NotNull = new Level1(),
                Null = null,
                Value = 1,
                Level1 = (Level1)Item
            };

            return this;
        }

        public ObjectExtensionsTestsFixture SetLevel1Index1()
        {
            Item = new Level1();

            Source = new Level0
            {
                NotNull = new Level1(),
                Null = null,
                Value = 1,
                Level1s = new List<Level1>
                {
                    new Level1(),
                    (Level1)Item
                }
            };

            return this;
        }

        public ObjectExtensionsTestsFixture SetLevel2()
        {
            Item = new Level2();

            Source = new Level0
            {
                NotNull = new Level1(),
                Null = null,
                Value = 1,
                Level1 = new Level1
                {
                    NotNull = new Level2(),
                    Null = new Level2(),
                    Value = 1,
                    Level2 = (Level2)Item
                }
            };

            return this;
        }

        public ObjectExtensionsTestsFixture SetIndex1Level2()
        {
            Item = new Level2();

            Source = new Level0
            {
                NotNull = new Level1(),
                Null = null,
                Value = 1,
                Level1s = new List<Level1>
                {
                    new Level1(),
                    new Level1
                    {
                        NotNull = new Level2(),
                        Null = new Level2(),
                        Value = 1,
                        Level2 = (Level2)Item
                    }
                }
            };

            return this;
        }

        public ObjectExtensionsTestsFixture SetLevel2Index1()
        {
            Item = new Level2();

            Source = new Level0
            {
                NotNull = new Level1(),
                Null = null,
                Value = 1,
                Level1 = new Level1
                {
                    NotNull = new Level2(),
                    Null = new Level2(),
                    Value = 1,
                    Level2s = new List<Level2>
                    {
                        new Level2(),
                        (Level2)Item
                    }
                }
            };

            return this;
        }

        public ObjectExtensionsTestsFixture SetLevel3()
        {
            Item = new Level3();

            Source = new Level0
            {
                NotNull = new Level1(),
                Null = null,
                Value = 1,
                Level1 = new Level1
                {
                    NotNull = new Level2(),
                    Null = null,
                    Value = 1,
                    Level2 = new Level2
                    {
                        NotNull = new Level3(),
                        Null = null,
                        Value = 1,
                        Level3 = (Level3)Item
                    }
                }
            };

            return this;
        }

        private class Level0
        {
            public Level1 NotNull { get; set; }
            public Level1 Null { get; set; }
            public int Value { get; set; }
            public Level1 Level1 { get; set; }
            public List<Level1> Level1s { get; set; }
        }

        private class Level1
        {
            public Level2 NotNull { get; set; }
            public Level2 Null { get; set; }
            public int Value { get; set; }
            public Level2 Level2 { get; set; }
            public List<Level2> Level2s { get; set; }
        }

        private class Level2
        {
            public Level3 NotNull { get; set; }
            public Level3 Null { get; set; }
            public int Value { get; set; }
            public Level3 Level3 { get; set; }
            public List<Level3> Level3s { get; set; }
        }

        private class Level3
        {
        }
    }
}