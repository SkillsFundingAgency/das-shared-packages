using FluentAssertions;
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
            Test(f => f.SetRuleName(51), f => f.Shorten(), (f, r) => r.Should().NotBeNull().And.BeOfType<string>().Which.Length.Should().BeLessOrEqualTo(50));
        }

        [TestCase(25)]
        [TestCase(49)]
        [TestCase(50)]
        public void Shorten_WhenRuleNameIsLessThanOrEqualTo50Characters_ThenShouldReturnUnshortenedRuleName(int ruleNameLength)
        {
            Test(f => f.SetRuleName(ruleNameLength), f => f.Shorten(), (f, r) => r.Should().Be(f.RuleName));
        }

        [Test]
        public void Shorten_WhenRuleNameIsGreaterThan50Characters_ThenShouldReturnDifferentShortenedRuleNamesForDifferentRuleNames()
        {
            Test(f => f.SetMultipleRuleNames(51, true), f => f.ShortenMultiple(), (f, r) => r[0].Should().NotBe(r[1]));
        }

        [Test]
        public void Shorten_WhenRuleNameIsGreaterThan50Characters_ThenShouldReturnSameShortenedRuleNamesForSameRuleNames()
        {
            Test(f => f.SetMultipleRuleNames(51, false), f => f.ShortenMultiple(), (f, r) => r[0].Should().Be(r[1]));
        }
    }

    public class RuleNameShortenerTestsFixture
    {
        public string RuleName { get; set; }
        public string[] RuleNames { get; set; }
        public RuleNameShortener RuleNameShortener { get; set; }
        
        public RuleNameShortenerTestsFixture()
        {
            RuleNameShortener = new RuleNameShortener();
        }

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
            RuleName = new string('0', ruleNameLength);

            return this;
        }

        public RuleNameShortenerTestsFixture SetMultipleRuleNames(int ruleNameLength, bool uniqueRuleNames)
        {
            RuleNames = new[]
            {
                RuleNameShortener.Shorten(new string('0', ruleNameLength)),
                RuleNameShortener.Shorten(new string(uniqueRuleNames ? '1' : '0', ruleNameLength))
            };
            
            return this;
        }
    }
}