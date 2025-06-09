using System;

namespace SFA.DAS.AcademicYearService;
public static class AcademicYearHelper
{
    public static bool IsInAcademicYear(this DateTime date, int academicYear)
    {
        if (!ConvertAcademicYear(academicYear, out var startYear, out var finishYear))
            return false;

        return date >= new DateTime(startYear, 8, 1) && date < new DateTime(finishYear, 8, 1);
    }

    public static bool IsValidAcademicYear(this int academicYear)
    {
        return ConvertAcademicYear(academicYear, out _, out _);
    }

    private static bool ConvertAcademicYear(int academicYears, out int startYear, out int finishYear)
    {
        startYear = academicYears / 100 + 2000;
        finishYear = academicYears % 100 + 2000;

        if (academicYears <= 0 || academicYears > 9999)
        {
            return false;
        }

        if (startYear + 1 == finishYear)
        {
            return true;
        }

        if (startYear == 2099 && finishYear == 2000)
        {
            finishYear = 2100;
            return true;
        }

        return false;
    }
}