using System;

namespace SFA.DAS.AcademicYearService
{
    public static class AcademicYearHelper
    {
        public static bool IsInAcademicYear(this DateTime date, int academicYear)
        {
            var (startYear, finishYear) = ConvertAcademicYear(academicYear);

            return date >= new DateTime(startYear, 7, 1) && date < new DateTime(finishYear, 7, 1);
        }

        private static (int, int) ConvertAcademicYear(int academicYears)
        {
            if (academicYears < 2000 || academicYears > 3000)
            {
                throw new ArgumentException($"Invalid academic years {academicYears}");
            }

            int firstAcademicYear = academicYears / 100;
            int secondAcademicYear = academicYears % 100;

            if(firstAcademicYear + 1 != secondAcademicYear)
            {
                throw new ArgumentException($"Invalid academic years {academicYears}");
            }

            return (2000 + firstAcademicYear, 2000 + secondAcademicYear);
        }
    }
}
