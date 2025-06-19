using System;
using System.Collections;

namespace SFA.DAS.AcademicYearService.UnitTests;

internal class ValidAcademicYearCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        yield return new object[] { new DateTime(2000, 08, 02), 0001 };
        yield return new object[] { new DateTime(2001, 01, 22), 0001 };
        yield return new object[] { new DateTime(2024, 08, 01), 2425 };
        yield return new object[] { new DateTime(2024, 09, 15), 2425 };
        yield return new object[] { new DateTime(2025, 01, 01), 2425 };
        yield return new object[] { new DateTime(2025, 07, 31), 2425 };
        yield return new object[] { new DateTime(2099, 08, 01), 9900 };
        yield return new object[] { new DateTime(2100, 05, 01), 9900 };
    }
}

internal class InvalidAcademicYearCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        yield return new object[] { new DateTime(2024, 01, 01), 2526 };
        yield return new object[] { new DateTime(2025, 07, 31), 2526 };
        yield return new object[] { new DateTime(2026, 08, 01), 2526 };
        yield return new object[] { new DateTime(2026, 08, 01), 2628 };
        yield return new object[] { new DateTime(2026, 08, 01), 2625 };
        yield return new object[] { new DateTime(1999, 08, 01), 9900 };
        yield return new object[] { new DateTime(2199, 08, 01), 9900 };
        yield return new object[] { new DateTime(1001, 06, 01), 0001 };
    }
}