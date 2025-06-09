using System;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.AcademicYearService.UnitTests;

public class AcademicYearTests
{
    [TestCaseSource(typeof(ValidAcademicYearCases))]
    public void DateShouldBeWithinAcademicYear(DateTime date, int academicYear)
        => date.IsInAcademicYear(academicYear).Should().BeTrue();

    [TestCaseSource(typeof(InvalidAcademicYearCases))]
    public void DateShouldNotBeWithinAcademicYear(DateTime date, int academicYear)
        => date.IsInAcademicYear(academicYear).Should().BeFalse();

    [TestCase(0001, true)]
    [TestCase(0100, false)]
    [TestCase(2523, false)]
    [TestCase(2324, true)]
    [TestCase(9900, true)]
    [TestCase(0099, false)]
    [TestCase(0000, false)]
    [TestCase(-1, false)]
    [TestCase(10001, false)]
    public void FormatOfAcademicYearCheck(int academicYear, bool isValid)
        => academicYear.IsValidAcademicYear().Should().Be(isValid);
}
