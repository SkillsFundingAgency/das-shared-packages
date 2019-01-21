using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.AzureServiceBus
{
    [TestFixture]
    [Parallelizable]
    public class RuleNameShortenerTests : FluentTest<RuleNameShortenerTestsFixture>
    {
        [Test]
        public void Shorten_WhenRuleNameIsGreaterThan50Characters_ThenShouldReturnShortenedRuleName()
        {
            Run(f => f.SetRuleNameLength(51), f => f.Shorten(), (f, r) => r.Should().NotBeNull().And.BeOfType<string>().Which.Length.Should().BeLessOrEqualTo(50));
        }
        
        [Test]
        public void Shorten_WhenRuleNameIsEqualTo50Characters_ThenShouldReturnUnshortenedRuleName()
        {
            Run(f => f.SetRuleNameLength(50), f => f.Shorten(), (f, r) => r.Should().Be(f.RuleName));
        }
        
        [Test]
        public void Shorten_WhenRuleNameIsLessThan50Characters_ThenShouldReturnUnshortenedRuleName()
        {
            Run(f => f.SetRuleNameLength(50), f => f.Shorten(), (f, r) => r.Should().Be(f.RuleName));
        }

        [Test]
        public void Shorten_WhenRuleNameIsGreaterThan50Characters_ThenShouldReturnUniqueShortenedRuleName()
        {
            Run(f => f.ShortenMultiple(), (f, r) =>
            {
                r[0].Should().NotBe(r[1]);
                r[1].Should().Be(r[2]);
            });
        }
    }

    public class RuleNameShortenerTestsFixture
    {
        public string RuleName { get; set; }
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
                RuleNameShortener.Shorten(new string('0', 51)),
                RuleNameShortener.Shorten(new string('1', 51)),
                RuleNameShortener.Shorten(new string('1', 51))
            };
        }

        public RuleNameShortenerTestsFixture SetRuleNameLength(int length)
        {
            RuleName = new string('0', length);

            return this;
        }
    }
}