using System;

namespace SFA.DAS.Provider.Shared.UI.Models.Flags
{
    /// <summary>
    /// Flags for displaying banners on the apprentice details page. Note that these are bit flags and should be powers of 2 (achieved by using the left shift operator).
    /// </summary>
    [Flags]
    public enum ApprenticeDetailsBanners : ulong
    {
        None = 0,
        ChangeOfStartDateSent = 1 << 0,
        ChangeOfStartDateApproved = 1 << 1,
        ChangeOfStartDateCancelled = 1 << 2,
        ChangeOfPriceRequestSent = 1 << 3,
        ChangeOfPriceCancelled = 1 << 4,
        ChangeOfPriceApproved = 1 << 5,
        ChangeOfPriceAutoApproved = 1 << 6,
        ChangeOfPriceRejected = 1 << 7
    }
}
