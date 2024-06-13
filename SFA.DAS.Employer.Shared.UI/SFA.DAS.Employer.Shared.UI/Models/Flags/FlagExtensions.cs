namespace SFA.DAS.Employer.Shared.UI.Models.Flags
{
    public static class FlagExtensions
    {
        public static bool IsFlagSet(this ApprenticeDetailsBanners banners, ApprenticeDetailsBanners flag)
        {
            return (banners & flag) == flag;
        }
    }
}