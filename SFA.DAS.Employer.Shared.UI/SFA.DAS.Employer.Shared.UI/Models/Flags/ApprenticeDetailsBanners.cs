using System;

namespace SFA.DAS.Employer.Shared.UI.Models.Flags
{
    /// <summary>
    /// Flags for displaying banners on the apprentice details page. Note that these are bit flags and should be powers of 2 (achieved by using the left shift operator).
    /// </summary>
    [Flags]
    public enum ApprenticeDetailsBanners : ulong
    {
        None = 0,
        ChangeOfPriceRejected = 1 << 0,
        ChangeOfPriceApproved = 1 << 1,
        ChangeOfPriceRequestSent = 1 << 2,
        ChangeOfPriceCancelled = 1 << 3,
        ChangeOfStartDateApproved = 1 << 4,
        ChangeOfStartDateRejected = 1 << 5,
        ProviderPaymentsInactive = 1 << 6
    }
}
