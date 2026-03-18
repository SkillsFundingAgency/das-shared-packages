using SFA.DAS.SharedOuterApi.Extensions;

namespace SFA.DAS.SharedOuterApi.UnitTests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [TestCase("VAC12345", "12345")]
        [TestCase("vac12345", "12345")]
        [TestCase("12345VAC", "12345")]
        [TestCase("12345vac", "12345")]
        [TestCase("VAC12345VAC", "12345")]
        [TestCase("vac12345vac", "12345")]
        public void TrimVacancyReference_ShouldReturnExpectedResult(string input, string expected)
        {
            var result = input.TrimVacancyReference();
            result.Should().Be(expected);
        }
    }
}
