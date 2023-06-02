using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Enums
{
    public class CustomServiceRoleValueTypeTest
    {
        [TestCaseSource(nameof(CustomServiceRoleEnumValues))]
        public void Then_The_Properties_Are_Correctly_Returned(CustomServiceRoleValueType roleValue)
        {
            // assert
            Assert.That(Convert.ToInt32(roleValue) > 0, Is.EqualTo(true));
        }

        private static IEnumerable<object[]> CustomServiceRoleEnumValues()
        {
            return from object? number in Enum.GetValues(typeof(CustomServiceRoleValueType)) select new object[] { number };
        }
    }
}
