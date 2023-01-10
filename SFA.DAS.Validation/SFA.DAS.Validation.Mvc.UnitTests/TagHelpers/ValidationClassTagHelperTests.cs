using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NUnit.Framework;
using SFA.DAS.Testing;
using SFA.DAS.Validation.Mvc.TagHelpers;

namespace SFA.DAS.Validation.Mvc.UnitTests.TagHelpers
{
    [TestFixture]
    public class ValidationClassTagHelperTests : FluentTest<ValidationClassTagHelperTestsFixture>
    {
        [Test]
        public void Process_WhenModelStateIsInvalid_ThenShouldAddValidationClass()
        {
            Run(
                f => f.SetInvalidModelState(),
                f => f.Process(),
                f => f.TagHelperOutput.Attributes["class"].Value.Should().BeOfType<string>().Which.Should().Contain(f.ValidationClass));
        }
        
        [Test]
        public void Process_WhenModelStateIsValid_ThenShouldNotAddValidationClass()
        {
            Run(
                f => f.Process(),
                f => f.TagHelperOutput.Attributes["class"]?.Value.Should().BeOfType<string>().Which.Should().NotContain(f.ValidationClass));
        }
    }

    public class ValidationClassTagHelperTestsFixture
    {
        public string Model { get; set; }
        public string ValidationClass { get; set; }
        public IModelMetadataProvider MetadataProvider { get; set; }
        public ModelExplorer ModelExplorer { get; set; }
        public ModelExpression ModelExpression { get; set; }
        public ViewContext ViewContext { get; set; }
        public TagHelper TagHelper { get; set; }
        public TagHelperOutput TagHelperOutput { get; set; }

        public ValidationClassTagHelperTestsFixture()
        {
            Model = "Foobar";
            ValidationClass = "govuk-input--error";
            MetadataProvider = new EmptyModelMetadataProvider();
            ModelExplorer = MetadataProvider.GetModelExplorerForType(typeof(string), Model);
            ModelExpression = new ModelExpression("", ModelExplorer);
            ViewContext = new ViewContext();
            
            TagHelper = new ValidationClassTagHelper
            {
                For = ModelExpression,
                ValidationClass = ValidationClass,
                ViewContext = ViewContext
            };
            
            TagHelperOutput = new TagHelperOutput(
                "input",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(null));
        }

        public void Process()
        {
            TagHelper.Process(null, TagHelperOutput);
        }

        public ValidationClassTagHelperTestsFixture SetInvalidModelState()
        {
            ViewContext.ModelState.AddModelError(ModelExpression.Name, "Oops!");
            
            return this;
        }
    }
}
