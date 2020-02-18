using FluentAssertions;
using NUnit.Framework;
using System;

namespace SFA.DAS.MA.Shared.UI.UnitTest.LinkHelper
{
    [TestFixture]
    public class WhenRenderListItemLink
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
        public void ThenTheListItemContentIsRendered()
        {
            // arrange
            var content = Guid.NewGuid().ToString();
            _linkCollection.AddOrUpdateLink(new TestLink(content));

            // act
            var result = _sut.RenderListItemLink<TestLink>();

            // assert            
            result.Should().Be($"<li>{content}</li>");
        }
    }
}
