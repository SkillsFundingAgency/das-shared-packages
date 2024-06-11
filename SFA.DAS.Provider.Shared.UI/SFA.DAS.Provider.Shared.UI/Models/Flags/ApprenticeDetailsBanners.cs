using System;

namespace SFA.DAS.Provider.Shared.UI.Models.Flags;

/// <summary>
/// Flags for displaying banners on the apprentice details page. Note that these are bit flags and should be powers of 2.
/// </summary>
[Flags]
public enum ApprenticeDetailsBanners : ulong
{
    None = 0,
    ChangeOfStartDateSent = 1,
    ChangeOfStartDateApproved = 2,
    ChangeOfStartDateCancelled = 4,
    ChangeOfPriceRequestSent = 8,
    PriceChangeCancelled = 16,
    PriceChangeApproved = 32,
    ChangeOfPriceAutoApproved = 64,
    PriceChangeRejected = 128
}
