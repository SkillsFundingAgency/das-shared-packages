using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.MA.Shared.UI.UnitTest.LinkCollection;
using System;

namespace SFA.DAS.MA.Shared.UI.UnitTest.LinkHelper
{
    [TestFixture]
    public class WhenRenderLink
    {
        private Services.LinkHelper _sut;
        private Models.LinkCollection _linkCollection;

        [SetUp]
        public void Setup()
        {
            _linkCollection = new Models.LinkCollection();
            _sut = new Services.LinkHelper(_linkCollection);
        }

        [Test]
        public void ThenTheLinkContentisRendered()
        {
            // arrange
            var content = Guid.NewGuid().ToString();
            _linkCollection.AddOrUpdateLink(new TestLink(content));

            // act
            var result = _sut.RenderLink<TestLink>();

            // assert            
            result.Should().Be(content);
        }

        [Test]
        public void ThenWhenIsSelectedTheLinkContentisRenderedAsSelected()
        {
            // arrange
            var content = Guid.NewGuid().ToString();
            _linkCollection.AddOrUpdateLink(new TestLink(content));

            // act
            var result = _sut.RenderLink<TestLink>(null, null, true);

            // assert            
            result.Should().Be("IsSelected" + content);
        }

        [Test]
        public void ThenWhenBeforeAndAfterTextIsIncluudedTheLinkContentisRenderedBetweenTheText()
        {
            // arrange
            var beforeContent = Guid.NewGuid().ToString();
            var content = Guid.NewGuid().ToString();
            var afterContent = Guid.NewGuid().ToString();
            _linkCollection.AddOrUpdateLink(new TestLink(content));

            // act
            var result = _sut.RenderLink<TestLink>(() => beforeContent, () => afterContent);

            // assert            
            result.Should().Be($"{beforeContent}{content}{afterContent}");
        }

        [Test]
        public void ThenWhenTheLinkTypeisNotIntheCollectiontheResultIsAnEmptyString()
        {
            // arrange
            var content = Guid.NewGuid().ToString();
            _linkCollection.AddOrUpdateLink(new TestLink2(content));

            // act
            var result = _sut.RenderLink<TestLink>();

            // assert            
            result.Should().BeEmpty();
        }
    }
}
