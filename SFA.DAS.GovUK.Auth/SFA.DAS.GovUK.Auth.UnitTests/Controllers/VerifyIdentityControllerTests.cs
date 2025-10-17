using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.GovUK.Auth.Controllers;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.UnitTests.Controllers
{
    [TestFixture]
    public class VerifyIdentityControllerTests
    {
        private Mock<IGovUkAuthenticationService> _authServiceMock;
        private VerifyIdentityController _sut;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IGovUkAuthenticationService>();
            _sut = new VerifyIdentityController(_authServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            if(_sut != null)
                _sut.Dispose();
        }

        [Test]
        public async Task Index_Calls_ChallengeWithVerifyAsync_With_ReturnUrl_And_Controller()
        {
            // Arrange
            var returnUrl = "/somewhere";
            var expectedResult = new RedirectResult("/auth");

            _authServiceMock
                .Setup(s => s.ChallengeWithVerifyAsync(returnUrl, _sut))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _sut.Index(returnUrl);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _authServiceMock.Verify(s => s.ChallengeWithVerifyAsync(returnUrl, _sut), Times.Once);
        }
    }
}
