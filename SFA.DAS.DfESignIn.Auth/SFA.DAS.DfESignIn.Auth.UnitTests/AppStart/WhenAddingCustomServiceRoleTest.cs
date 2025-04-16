using Moq;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.AppStart
{
    public class WhenAddingCustomServiceRoleTest
    {
        [TestCase(CustomServiceRoleValueType.Code, null)]
        [TestCase(CustomServiceRoleValueType.Name, "http://schemas.portal.com/displayname")]
        public void Then_The_Properties_Are_Correctly_Returned(CustomServiceRoleValueType roleValue, string? roleClaim)
        {
            // arrange
            var customServiceRole = new Mock<ICustomServiceRole>();
            customServiceRole.Setup(c => c.RoleValueType).Returns(roleValue);
            customServiceRole.Setup(c => c.RoleClaimType).Returns(roleClaim ?? string.Empty);
            
            // assert
            Assert.That(customServiceRole.Object.RoleClaimType, Is.EqualTo(roleClaim ?? string.Empty));
        }
    }
}
