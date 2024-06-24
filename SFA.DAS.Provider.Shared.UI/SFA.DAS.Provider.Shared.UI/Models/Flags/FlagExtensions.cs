namespace SFA.DAS.Provider.Shared.UI.Models.Flags
{
    public static class FlagExtensions
    {
        public static bool IsFlagSet(this ApprenticeDetailsBanners banners, ApprenticeDetailsBanners flag)
        {
            return (banners & flag) == flag;
        }

        public static string AppendProviderBannersToUrl(this string url, params ApprenticeDetailsBanners[] banners)
        {
            ApprenticeDetailsBanners bannersCombined = 0;
            foreach (var banner in banners)
            {
                bannersCombined |= banner;
            }

            return $"{url}?banners={(ulong)bannersCombined}";
        }
    }
}