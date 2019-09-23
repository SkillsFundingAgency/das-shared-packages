using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Testing;
using Moq;
using SFA.DAS.BadLanguage.Data;
using SFA.DAS.BadLanguage.Service;
using FluentAssertions;

namespace SFA.DAS.BadLanguage.UnitTests
{
    [TestFixture]
    public class BadLanguageServiceTests : FluentTest<BadLanguageServiceTestsFixture>
    {
        [Test]
        public void WhenCheckingPhraseWithOneWordOfBadLanguage_ReturnsTrue()
        {
            Run(f => f.CheckBadLanguage(f.PhraseJustOneWordOfBadLanguage).Should().BeTrue());
        }

        [Test]
        public void WhenCheckingPhraseWithOneWordOfBadLanguageInADifferentCase_ReturnsTrue()
        {
            Run(f => f.CheckBadLanguage(f.PhraseJustOneWordOfBadLanguageDifferentCase).Should().BeTrue());
        }

        [Test]
        public void WhenCheckingPhraseWithBadLanguageAmongstNormalText_ReturnsTrue()
        {
            Run(f => f.CheckBadLanguage(f.PhraseBadLanguageAmongstNormalText).Should().BeTrue());
        }

        [Test]
        public void WhenCheckingPhraseThatIsAnEmptyString_ReturnsFalse()
        {
            Run(f => f.CheckBadLanguage(f.PhraseEmptyString).Should().BeFalse());
        }

        [Test]
        public void WhenCheckingPhraseWithoutBadLanguage_ReturnsFalse()
        {
            Run(f => f.CheckBadLanguage(f.PhraseWithoutBadLanguage).Should().BeFalse());
        }
    }

    public class BadLanguageServiceTestsFixture
    {
        public Mock<IBadLanguageRepository> BadLanguageRepository { get; set; }
        public string PhraseJustOneWordOfBadLanguage { get; set; }
        public string PhraseJustOneWordOfBadLanguageDifferentCase { get; set; }
        public string PhraseBadLanguageAmongstNormalText { get; set; }
        public string PhraseEmptyString { get; set; }
        public string PhraseWithoutBadLanguage { get; set; }
        public List<string> SampleBadLanguage { get; set; }
        public IBadLanguageService BadLanguageService { get; set; }

        public BadLanguageServiceTestsFixture()
        {
            BadLanguageRepository = new Mock<IBadLanguageRepository>();
            PhraseJustOneWordOfBadLanguage = "carrots";
            PhraseJustOneWordOfBadLanguageDifferentCase = "ceLERy";
            PhraseBadLanguageAmongstNormalText = "normal phrase cleanpotatoesss";
            PhraseEmptyString = string.Empty;
            PhraseWithoutBadLanguage = "Unrelated normal language";
            SampleBadLanguage = new List<string>
            {
                "Carrots",
                "Celery",
                "Potatoes"
            };
            BadLanguageService = new BadLanguageService(BadLanguageRepository.Object);
            BadLanguageRepository.Setup(x => x.GetBadLanguageList()).Returns(SampleBadLanguage);
        }

        public bool CheckBadLanguage(string phrase)
        {
            return BadLanguageService.ContainsBadLanguage(phrase);
        }
    }
}
