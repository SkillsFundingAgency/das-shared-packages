using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using FluentAssertions;

namespace SFA.DAS.Commitments.Api.Types.UnitTests
{
    [TestFixture]
    public class ApprenticeshipSearchResponseTests
    {
        [TestCase(100, 10, 10, Description = "When total is divisible exactly by page size")]
        [TestCase(101, 10, 11, Description = "When the last page has one item on")]
        [TestCase(99, 10, 10, Description = "When the last page has one item fewer than a full page")]
        [TestCase(105, 10, 11, Description = "When the last page has one item on")]
        public void TotalPageCountReturnsCorrectCount(int totalCount, int pageSize, int expectedTotalPages)
        {
            var query = new ApprenticeshipSearchResponse
            {
                TotalApprenticeships = totalCount,
                PageSize = pageSize
            };

            query.TotalPages.Should().Be(expectedTotalPages);
        }
    }
}
