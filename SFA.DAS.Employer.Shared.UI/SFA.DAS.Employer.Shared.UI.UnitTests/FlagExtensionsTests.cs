using FluentAssertions;
using SFA.DAS.Employer.Shared.UI.Models.Flags;

namespace SFA.DAS.Employer.Shared.UI.UnitTests
{
    [Parallelizable]
    public class FlagExtensionsTests
    {
        [TestCase(ApprenticeDetailsBanners.ChangeOfPriceApproved | ApprenticeDetailsBanners.ChangeOfStartDateRejected, ApprenticeDetailsBanners.ChangeOfStartDateRejected, true)]
        [TestCase(ApprenticeDetailsBanners.ChangeOfPriceApproved | ApprenticeDetailsBanners.ChangeOfStartDateRejected, ApprenticeDetailsBanners.ChangeOfPriceApproved, true)]
        [TestCase(ApprenticeDetailsBanners.ChangeOfPriceApproved | ApprenticeDetailsBanners.ChangeOfStartDateRejected, ApprenticeDetailsBanners.ChangeOfPriceRequestSent, false)]
        [TestCase(ApprenticeDetailsBanners.None, ApprenticeDetailsBanners.ChangeOfStartDateApproved, false)]
        public void IsFlagSet_ReturnsExpectedResult(ApprenticeDetailsBanners banners, ApprenticeDetailsBanners bannerFlag, bool expectedToBeSet)
        {
            var result = banners.IsFlagSet(bannerFlag);
            result.Should().Be(expectedToBeSet);
        }

        [TestCase(ApprenticeDetailsBanners.ChangeOfPriceApproved | ApprenticeDetailsBanners.ChangeOfStartDateRejected, (ulong)68)]
        [TestCase(ApprenticeDetailsBanners.ChangeOfPriceRequestSent, (ulong)8)]
        public void AppendProviderBannersToUrl_ReturnsExpectedResult(ApprenticeDetailsBanners banners, ulong expectedBannerValue)
        {
            var result = Guid.NewGuid().ToString().AppendEmployerBannersToUrl(banners);
            result.Should().EndWith($"?banners={expectedBannerValue}");
        }
    }
}