using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Models;

namespace SFA.DAS.MA.Shared.UI.UnitTests.HeaderViewModel
{
    [TestFixture]
    public class WhenHideMenu
    {
        private IHeaderViewModel _sut;
        private Mock<IHeaderConfiguration> _mockHeaderConfiguration;
        private Mock<IUserContext> _mockUserContext;

        [SetUp]
        public void Setup()
        {
            _mockHeaderConfiguration = new Mock<IHeaderConfiguration>();
            _mockUserContext = new Mock<IUserContext>();

            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);
        }

        [Test]
        public void ThenTheMenuIsHiddenPropertyIsSet()
        {
            // arrange
            _sut.MenuIsHidden.Should().BeFalse();

            // act
            _sut.HideMenu();

            // assert            
            _sut.MenuIsHidden.Should().BeTrue();
        }
    }
}
