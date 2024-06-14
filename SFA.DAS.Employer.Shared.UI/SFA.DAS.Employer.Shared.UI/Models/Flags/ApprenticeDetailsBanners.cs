using System;

namespace SFA.DAS.Employer.Shared.UI.Models.Flags
{
    [Flags]
    public enum ApprenticeDetailsBanners : ulong
    {
        None = 0,
        ChangeOfPriceRejected = 2,
        ChangeOfPriceApproved = 4,
        ChangeOfPriceRequestSent = 8,
        ChangeOfPriceCancelled = 16,
        ChangeOfStartDateApproved = 32,
        ChangeOfStartDateRejected = 64,
        ProviderPaymentsInactive = 128
    }
}
