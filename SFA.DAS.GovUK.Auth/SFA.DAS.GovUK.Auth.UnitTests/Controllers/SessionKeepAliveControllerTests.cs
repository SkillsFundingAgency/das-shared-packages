using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.Auth.Controllers;

namespace SFA.DAS.GovUK.Auth.UnitTests.Controllers;

public class SessionKeepAliveControllerTests
{
    [Test]
    public void KeepAlive_ShouldReturnNoContent()
    {
        // Arrange
        var controller = new SessionKeepAliveController();
        
        // Act
        var response = controller.Index();
        
        // Assert
        response.Should().BeOfType<NoContentResult>();
    }
}