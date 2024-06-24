using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Provider.Shared.UI.Models.Flags;

namespace SFA.DAS.Provider.Shared.UI.UnitTests.Extensions;

[Parallelizable]
public class FlagExtensionsTests
{
    [TestCase(ApprenticeDetailsBanners.ChangeOfPriceApproved | ApprenticeDetailsBanners.ChangeOfPriceAutoApproved, ApprenticeDetailsBanners.ChangeOfPriceAutoApproved, true)]
    [TestCase(ApprenticeDetailsBanners.ChangeOfPriceApproved | ApprenticeDetailsBanners.ChangeOfPriceAutoApproved, ApprenticeDetailsBanners.ChangeOfPriceApproved, true)]
    [TestCase(ApprenticeDetailsBanners.ChangeOfPriceApproved | ApprenticeDetailsBanners.ChangeOfPriceAutoApproved, ApprenticeDetailsBanners.ChangeOfPriceRequestSent, false)]
    [TestCase(ApprenticeDetailsBanners.None, ApprenticeDetailsBanners.ChangeOfStartDateApproved, false)]
    public void IsFlagSet_ReturnsExpectedResult(ApprenticeDetailsBanners banners, ApprenticeDetailsBanners bannerFlag, bool expectedToBeSet)
    {
        var result = banners.IsFlagSet(bannerFlag);
        result.Should().Be(expectedToBeSet);
    }

    [TestCase(ApprenticeDetailsBanners.ChangeOfPriceApproved | ApprenticeDetailsBanners.ChangeOfPriceAutoApproved, (ulong)96)]
    [TestCase(ApprenticeDetailsBanners.ChangeOfPriceRequestSent, (ulong)8)]
    public void AppendProviderBannersToUrl_ReturnsExpectedResult(ApprenticeDetailsBanners banners, ulong expectedBannerValue)
    {
        var result = Guid.NewGuid().ToString().AppendProviderBannersToUrl(banners);
        result.Should().EndWith($"?banners={expectedBannerValue}");
    }
}