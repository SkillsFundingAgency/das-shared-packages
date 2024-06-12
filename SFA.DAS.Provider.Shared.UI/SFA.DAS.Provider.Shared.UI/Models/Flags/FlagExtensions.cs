namespace SFA.DAS.Provider.Shared.UI.Models.Flags
{
    public static class FlagExtensions
    {
        public static bool IsFlagSet(this ApprenticeDetailsBanners banners, ApprenticeDetailsBanners flag)
        {
            return (banners & flag) == flag;
        }
    }
}