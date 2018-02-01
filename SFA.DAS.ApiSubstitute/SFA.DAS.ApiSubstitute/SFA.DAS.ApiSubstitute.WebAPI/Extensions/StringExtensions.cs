
namespace SFA.DAS.ApiSubstitute.WebAPI.Extensions
{
    public static class StringExtensions
    {
        public static string GetEndPoint(this string endPoint)
        {
            return endPoint.StartsWith("/")
                ? endPoint.TrimStart('/')
                : endPoint;
        }
        public static string GetBaseUrl(this string baseAddress)
        {
            return baseAddress.EndsWith("/")
                ? baseAddress
                : baseAddress + "/";
        }
    }
}
