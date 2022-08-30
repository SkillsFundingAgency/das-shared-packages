using NUnit.Framework;
using FluentAssertions;

namespace SFA.DAS.MA.Shared.UI.UnitTests.LinkCollection
{
    [TestFixture]
    public class WhenRemoveLink
    {
        private Models.LinkCollection _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new Models.LinkCollection();
            _sut.AddOrUpdateLink(new TestLink());
            _sut.Links.Count.Should().Be(1);
        }

        [Test]
        public void ThenTheLinkisRemovedFromTheCollection()
        {
            // arrange
            _sut.Links.Count.Should().Be(1);

            // act
            _sut.RemoveLink<TestLink>();

            // assert            
            _sut.Links.Count.Should().Be(0);
        }

        [Test]
        public void ThenWithAnEmptyCollectionTheCollectionIsStillEmpty()
        {
            // arrange
            _sut = new Models.LinkCollection();
            _sut.Links.Count.Should().Be(0);

            // act
            _sut.RemoveLink<TestLink>();

            // assert            
            _sut.Links.Count.Should().Be(0);
        }
    }
}
