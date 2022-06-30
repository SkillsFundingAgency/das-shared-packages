using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Models;
using System;

namespace SFA.DAS.MA.Shared.UI.UnitTests.HeaderViewModel
{
    [TestFixture]
    public class WhenSelectMenu
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
        public void ThenTheSelectedMenuPropertyIsSet()
        {
            // arrange
            _sut.SelectedMenu.Should().Be("home");
            var menu = Guid.NewGuid().ToString();

            // act
            _sut.SelectMenu(menu);

            // assert            
            _sut.SelectedMenu.Should().Be(menu);
        }
    }
}
