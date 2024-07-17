using FluentAssertions;
using FluentValidation;
using SFA.DAS.InputValidation.Fluent.Extensions;

namespace SFA.DAS.InputValidation.UnitTests.Fluent;

public class FreeTextValidatorTests
{
    private SomeTestClassValidator? _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new SomeTestClassValidator();
    }

    [TestCase("test",true)]
    [TestCase("?@!\"'/",true)]
    [TestCase("<test>",false)]
    public void ThenValidatesInput(string input, bool isValid)
    {
        var model = new SomeTestClass
        {
            MyTestString = input
        };
        var actual = _validator!.Validate(model);

        actual.IsValid.Should().Be(isValid);
        if (!actual.IsValid)
        {
            actual.Errors
                .FirstOrDefault(c => c.ErrorMessage == "My Test String must only include letters a to z, numbers 0 to 9, and special characters such as hyphens, spaces and apostrophes")
                .Should().NotBeNull();
        }
    }

    private class SomeTestClassValidator : AbstractValidator<SomeTestClass>
    {
        public SomeTestClassValidator()
        {
            RuleFor(x => x.MyTestString)
                .ValidFreeTextCharacters().WithErrorCode(nameof(SomeTestClass.MyTestString));
        }
    }

    private class SomeTestClass
    {
        public string? MyTestString { get; set; }
    }
}