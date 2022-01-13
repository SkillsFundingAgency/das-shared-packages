using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.EmailValidationService.UnitTests
{
    public class EmailValidationTests
    {
        [TestCaseSource(typeof(ValidEmailCases))]
        public void EmailShouldBeValid(string email)
            => email.IsAValidEmailAddress().Should().BeTrue();

        [TestCaseSource(typeof(InvalidEmailCases))]
        public void EmailShouldBeInvalid(string email)
            => email.IsAValidEmailAddress().Should().BeFalse();

        [TestCaseSource(typeof(ValidEmailCases))]
        public void EmailShouldBeValidInDataAttribute(string email)
            => new EsfaEmailAddressAttribute().IsValid(email).Should().BeTrue();

        [TestCaseSource(typeof(InvalidEmailCases))]
        public void EmailShouldBeInvalidInDataAttribute(string email)
            => new EsfaEmailAddressAttribute().IsValid(email).Should().BeFalse();
    }
}