using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.Configuration.AzureServiceBus
{
    [TestFixture]
    [Parallelizable]
    public class RuleNameShortenerTests : FluentTest<RuleNameShortenerTestsFixture>
    {
        [Test]
        public void Shorten_WhenRuleNameIsGreaterThan50Characters_ThenShouldReturnShortenedRuleName()
        {
            Test(f => f.SetRuleName(51), f => f.Shorten(), (_, r) => r.Should().NotBeNull().And.BeOfType<string>().Which.Length.Should().BeLessOrEqualTo(50));
        }

        [TestCase(25)]
        [TestCase(49)]
        [TestCase(50)]
        public void Shorten_WhenRuleNameIsLessThanOrEqualTo50Characters_ThenShouldReturnUnshortenedRuleName(int ruleNameLength)
        {
            Test(f => f.SetRuleName(ruleNameLength), f => f.Shorten(), (f, r) => r.Should().Be(f.RuleName.FullName));
        }

        [Test]
        public void Shorten_WhenRuleNameIsGreaterThan50Characters_ThenShouldReturnDifferentShortenedRuleNamesForDifferentRuleNames()
        {
            Test(f => f.SetMultipleRuleNames(51, true), f => f.ShortenMultiple(), (_, r) => r[0].Should().NotBe(r[1]));
        }

        [Test]
        public void Shorten_WhenRuleNameIsGreaterThan50Characters_ThenShouldReturnSameShortenedRuleNamesForSameRuleNames()
        {
            Test(f => f.SetMultipleRuleNames(51, false), f => f.ShortenMultiple(), (_, r) => r[0].Should().Be(r[1]));
        }
    }

    public class RuleNameShortenerTestsFixture
    {
        public Type RuleName { get; set; }
        public Type[] RuleNames { get; set; }

        public string Shorten()
        {
            return RuleNameShortener.Shorten(RuleName);
        }

        public string[] ShortenMultiple()
        {
            return new[]
            {
                RuleNameShortener.Shorten(RuleNames[0]),
                RuleNameShortener.Shorten(RuleNames[1])
            };
        }

        public RuleNameShortenerTestsFixture SetRuleName(int ruleNameLength)
        {
            var ruleType = new Mock<Type>();
            var typeFullName = new string('x', ruleNameLength);
            ruleType.SetupGet(rt => rt.FullName).Returns(typeFullName);

            RuleName = ruleType.Object;

            return this;
        }

        public RuleNameShortenerTestsFixture SetMultipleRuleNames(int ruleNameLength, bool uniqueRuleNames)
        {
            var ruleType1 = new Mock<Type>();
            var type1FullName = new string('0', ruleNameLength);
            ruleType1.SetupGet(rt => rt.FullName).Returns(type1FullName);
            RuleName = ruleType1.Object;

            var ruleType2 = new Mock<Type>();
            var type2FullName = new string('1', ruleNameLength);
            ruleType2.SetupGet(rt => rt.FullName).Returns(type2FullName);

            RuleNames = new[]
            {
                ruleType1.Object,
                uniqueRuleNames ? ruleType2.Object : ruleType1.Object
            };
            
            return this;
        }
    }
}