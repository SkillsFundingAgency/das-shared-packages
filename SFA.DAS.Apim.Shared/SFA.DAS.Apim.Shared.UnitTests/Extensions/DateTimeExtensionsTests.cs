using System;
using SFA.DAS.Apim.Shared.Extensions;

namespace SFA.DAS.Apim.Shared.UnitTests.Extensions;
[TestFixture]
internal class DateTimeExtensionsTests
{
    [Test]
    public void ToDayMonthYearString_ShouldReturnEmpty_WhenDateIsNull()
    {
        // Arrange
        DateTime? date = null;

        // Act
        var result = date.ToDayMonthYearString();

        // Assert
        result.Should().BeNullOrEmpty();
    }

    [Test]
    public void ToDayMonthYearString_ShouldFormatDateCorrectly_ForValidDate()
    {
        // Arrange
        DateTime? date = new DateTime(2025, 9, 8); // 8th September 2025

        // Act
        var result = date.ToDayMonthYearString();

        // Assert
        result.Should().Be("8 September 2025");
    }

    [Theory]
    [MoqInlineAutoData(2024, 1, 1, "1 January 2024")]
    [MoqInlineAutoData(2024, 12, 31, "31 December 2024")]
    [MoqInlineAutoData(2024, 2, 29, "29 February 2024")] // leap year
    public void ToDayMonthYearString_ShouldFormatVariousDates(int year, int month, int day, string expected)
    {
        // Arrange
        DateTime? date = new DateTime(year, month, day);

        // Act
        var result = date.ToDayMonthYearString();

        // Assert
        result.Should().Be(expected);
    }
}
