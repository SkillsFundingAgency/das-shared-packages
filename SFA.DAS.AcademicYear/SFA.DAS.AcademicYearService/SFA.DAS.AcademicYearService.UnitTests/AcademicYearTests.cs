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


    [TestCase(2524)]
    [TestCase(2024)]
    [TestCase(1999)]
    [TestCase(1999)]
    public void ShouldThrowExceptionForInvalidAcademicYear(int academicYear)
    {
        Action action = () => DateTime.Today.IsInAcademicYear(academicYear);
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Invalid academic years {academicYear}");
    }
}
