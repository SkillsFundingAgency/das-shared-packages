using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Shared.UI.Extensions;

namespace SFA.DAS.Provider.Shared.UI.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable]
    public class HttpHelperZendeskExtensionsTests
    {
        private HttpHelperZendeskExtensionsTestsFixture _fixture;
        [SetUp]
        public void Arrange()
        {
            _fixture = new HttpHelperZendeskExtensionsTestsFixture();
        }

        [Test]
        public void SetZendeskSuggestion_IsCreatedCorrectly()
        {
            var suggestion = "iowoiwueoiwue";
            var htmlSnippet = _fixture.Sut.SetZendeskSuggestion(suggestion);

            _fixture.ExpectedSuggestionJavaScriptSnippet(suggestion, htmlSnippet.ToString());
        }

        [Test]
        public void SetZendeskSuggestion_IsCreatedCorrectlyWithApostrophesEscaped()
        {
            var suggestion = "'help's";
            var htmlSnippet = _fixture.Sut.SetZendeskSuggestion(suggestion);

            _fixture.ExpectedSuggestionJavaScriptSnippet(@"\'help\'s", htmlSnippet.ToString());
        }

        [Test]
        public void SetZendeskLabels_IsCreatedCorrectlyForNoItems()
        {
            var labels = new String[0];
            var htmlSnippet = _fixture.Sut.SetZenDeskLabels(labels);

            _fixture.ExpectedLabelJavaScriptSnippet("''", htmlSnippet.ToString());
        }

        [Test]
        public void SetZendeskLabels_IsCreatedCorrectlyForOneItem()
        {
            var labels = new[] { "one" };
            var htmlSnippet = _fixture.Sut.SetZenDeskLabels(labels);

            _fixture.ExpectedLabelJavaScriptSnippet("'one'", htmlSnippet.ToString());
        }

        [Test]
        public void SetZendeskLabels_IsCreatedCorrectlyForTwoItems()
        {
            var labels = new[] { "one", "two" };
            var htmlSnippet = _fixture.Sut.SetZenDeskLabels(labels);

            _fixture.ExpectedLabelJavaScriptSnippet("'one','two'", htmlSnippet.ToString());
        }

        [Test]
        public void SetZendeskLabels_IsCreatedCorrectlyForOneItemWithApostrophesEscaped()
        {
            var labels = new[] { "one's" };
            var htmlSnippet = _fixture.Sut.SetZenDeskLabels(labels);

            _fixture.ExpectedLabelJavaScriptSnippet(@"'one\'s'", htmlSnippet.ToString());
        }
    }

    public class HttpHelperZendeskExtensionsTestsFixture
    {
        public Mock<IHtmlHelper> MockHtmlHelper;
        public IHtmlHelper Sut => MockHtmlHelper.Object;

        private const string StartLabelSnipet = "zE('webWidget', 'helpCenter:setSuggestions', { labels: [";
        private const string EndLabelSnipet = "] });";

        public HttpHelperZendeskExtensionsTestsFixture()
        {
            MockHtmlHelper = new Mock<IHtmlHelper>();
        }

        public void ExpectedSuggestionJavaScriptSnippet(string suggestion, string snippet)
        {
            Assert.AreEqual($"zE('webWidget', 'helpCenter:setSuggestions', {{ search: '{suggestion}' }});", snippet);
        }

        public void ExpectedLabelJavaScriptSnippet(string labels, string snippet)
        {
            var expected = StartLabelSnipet + labels + EndLabelSnipet;
            Assert.AreEqual(expected, snippet);
        }
    }
}