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
    }

    private class SomeTestClassValidator : AbstractValidator<SomeTestClass>
    {
        public SomeTestClassValidator()
        {
            RuleFor(x => x.MyTestString)
                .ValidFreeTextCharacters();
        }
    }

    private class SomeTestClass
    {
        public string? MyTestString { get; set; }
    }
}