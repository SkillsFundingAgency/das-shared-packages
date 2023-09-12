using FluentAssertions;
using SFA.DAS.DfESignIn.Auth.Extensions;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Extensions
{
    public class EnumExtensionTests
    {
        [Test]
        public void Then_Returns_Description_From_Attr()
        {
            EnumForTesting.ForTesting.GetDescription().Should().Be("Something for testing");
        }

        [Test]
        public void And_No_Attr_Then_Returns_Empty()
        {
            EnumForTesting.OopsNoDescription.GetDescription().Should().BeEmpty();
        }

        public enum EnumForTesting
        {
            [System.ComponentModel.Description("Something for testing")]
            ForTesting,
            OopsNoDescription
        }
    }
}