using System;
using System.Collections;

namespace SFA.DAS.AcademicYearService.UnitTests
{
    internal class ValidAcademicYearCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { new DateTime(2025, 01, 01), 2425 };
            yield return new object[] { new DateTime(2024, 07, 01), 2425 };
            yield return new object[] { new DateTime(2025, 06, 31), 2425 };
        }
    }

    internal class InvalidAcademicYearCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { new DateTime(2024, 01, 01), 2526 };
            yield return new object[] { new DateTime(2024, 01, 01), 2526 };
        }
    }
}