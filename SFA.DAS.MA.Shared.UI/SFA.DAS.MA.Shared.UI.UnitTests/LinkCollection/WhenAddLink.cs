using NUnit.Framework;
using FluentAssertions;

namespace SFA.DAS.MA.Shared.UI.UnitTests.LinkCollection
{
    [TestFixture]
    public class WhenAddLink
    {
        private Models.LinkCollection _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new Models.LinkCollection();
            _sut.Links.Should().BeEmpty();
        }

        [Test]
        public void ThenTheLinkisAddedtoTheCollection()
        {
            // arrange
            var testLink = new TestLink();

            // act
            _sut.AddOrUpdateLink(testLink);

            // assert            
            _sut.Links.Should().Contain(testLink);
        }

        [Test]
        public void ThenTheLinkisUpdatedWhenAlinkOfTheSameTypeAlreadyExistsInTheCollection()
        {
            // arrange
            _sut.AddOrUpdateLink(new TestLink());
            _sut.Links.Count.Should().Be(1);
            var testLink = new TestLink();

            // act
            _sut.AddOrUpdateLink(testLink);

            // assert            
            _sut.Links.Count.Should().Be(1);
            _sut.Links.Should().Contain(testLink);
        }

        [Test]
        public void ThenTheLinkisAddedToTheCollectionWhenItemsOfDifferentTypeAlreadyExist()
        {
            // arrange
            _sut.AddOrUpdateLink(new TestLink());
            _sut.Links.Count.Should().Be(1);
            var testLink = new TestLink2();

            // act
            _sut.AddOrUpdateLink(testLink);

            // assert            
            _sut.Links.Count.Should().Be(2);
            _sut.Links.Should().Contain(testLink);
        }
    }   
}
