using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.Validation.UnitTests
{
    [TestFixture]
    public class ValidationExceptionTests : FluentTest<ValidationExceptionTestsFixture>
    {
        [Test]
        public void Initialize_WhenInitializingWithNoMessageAndNoErrors_ThenShouldHaveDefaultProperties()
        {
            Run(f => f.Initialize(), (f, r) => r.Should().NotBeNull().And.Match<ValidationException>(e => e.Message == "" && !e.ValidationErrors.Any()));
        }

        [Test]
        public void Initialize_WhenInitializingWithAMessageAndNoErrors_ThenShouldHaveAMessageAndNoValidationErrors()
        {
            Run(f => f.SetMessage(), f => f.Initialize(), (f, r) => r.Should().NotBeNull().And.Match<ValidationException>(e => e.Message == f.Message && !e.ValidationErrors.Any()));
        }

        [Test]
        public void Initialize_WhenInitializingWithNoMessageAndErrors_ThenShouldHaveAnEmptyMessageAndValidationErrors()
        {
            Run(f => f.SetErrors(), f => f.Initialize(), (f, r) => r.Should().NotBeNull().And.Match<ValidationException>(e => e.Message == "" && e.ValidationErrors.Count() == f.Errors.Count));
        }

        [Test]
        public void Initialize_WhenInitializingWithMessageAndErrors_ThenShouldHaveAMessageAndValidationErrors()
        {
            Run(f => f.SetMessage().SetErrors(), f => f.Initialize(), (f, r) => r.Should().NotBeNull().And.Match<ValidationException>(e => e.Message == f.Message && e.ValidationErrors.Count() == f.Errors.Count));
        }

        [Test]
        public void Initialize_WhenInitializingWithErrors_ThenShouldHaveValidationErrorsForSpecificProperties()
        {
            Run(f => f.SetErrors(), f => f.Initialize(), (f, r) => r.ValidationErrors.ForEach(e => e.Should().Match<ValidationError>(e2 => e2.Property.Compile().DynamicInvoke(e2.Model) as string == e2.Message)));
        }
    }

    public class ValidationExceptionTestsFixture
    {
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public ValidationExceptionTestsFixture()
        {
            Errors = new List<string>();
        }

        public ValidationException Initialize()
        {
            var ex = Message == null ? new ValidationException() : new ValidationException(Message);
            
            for (var i = 0; i < Errors.Count; i++)
            {
                var i1 = i;
                ex.AddError(this, f => f.Errors[i1], Errors[i]);
            }

            return ex;
        }

        public ValidationExceptionTestsFixture SetErrors()
        {
            Errors.Add("Foo");
            Errors.Add("Bar");

            return this;
        }

        public ValidationExceptionTestsFixture SetMessage()
        {
            Message = "Oops!";

            return this;
        }
    }
}