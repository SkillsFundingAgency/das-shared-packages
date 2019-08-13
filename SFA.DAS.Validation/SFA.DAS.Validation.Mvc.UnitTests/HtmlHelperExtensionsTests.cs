#if NET462
using System.Web.Mvc;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.Validation.Mvc.UnitTests
{
    [TestFixture]
    public class HtmlHelperExtensionsTests : FluentTest<HtmlHelperExtensionsTestsFixture>
    {
        [Test]
        public void IsValid_WhenModelPropertyIsValid_ThenShouldReturnTrue()
        {
            Run(f => f.IsValid(), (f, r) => r.Should().BeTrue());
        }

        [Test]
        public void IsValid_WhenModelPropertyIsInvalid_ThenShouldReturnFalse()
        {
            Run(f => f.SetInvalidModelState(), f => f.IsValid(), (f, r) => r.Should().BeFalse());
        }

        [Test]
        public void ValidationClassFor_WhenModelPropertyIsValid_ThenShouldReturnEmpty()
        {
            Run(f => f.ValidationClassFor(), (f, r) => r.Should().Be(""));
        }

        [Test]
        public void ValidationClassFor_WhenModelPropertyIsValid_ThenShouldReturnClass()
        {
            Run(f => f.SetInvalidModelState(), f => f.ValidationClassFor(), (f, r) => r.Should().Be(f.ValidationClass));
        }
    }

    public class HtmlHelperExtensionsTestsFixture
    {
        public HtmlHelper<HtmlHelperExtensionsTestsFixture> HtmlHelper { get; set; }
        public string Prop { get; set; }
        public Mock<IViewDataContainer> ViewDataContainer { get; set; }
        public ViewDataDictionary ViewData { get; set; }
        public string ValidationClass { get; set; }

        public HtmlHelperExtensionsTestsFixture()
        {
            ViewDataContainer = new Mock<IViewDataContainer>();
            ViewData = new ViewDataDictionary();
            ValidationClass = "foobar";

            ViewDataContainer.Setup(c => c.ViewData).Returns(ViewData);

            HtmlHelper = new HtmlHelper<HtmlHelperExtensionsTestsFixture>(new ViewContext(), ViewDataContainer.Object);
        }

        public bool IsValid()
        {
            return HtmlHelper.IsValid(m => m.Prop);
        }

        public string ValidationClassFor()
        {
            return HtmlHelper.ValidationClassFor(m => m.Prop, ValidationClass);
        }

        public HtmlHelperExtensionsTestsFixture SetInvalidModelState()
        {
            ViewData.ModelState.AddModelError(nameof(Prop), "Oops!");

            return this;
        }
    }
}
#endif