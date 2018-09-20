#if NET462
using System.Web.Mvc;
    
namespace SFA.DAS.Validation.Mvc
{
    public static class TempDataDictionaryExtensions
    {
        public static T TryGet<T>(this TempDataDictionary tempData) where T : class
        {
            var key = typeof(T).FullName;

            if (tempData.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return null;
        }

        public static void Set<T>(this TempDataDictionary tempData, T value) where T : class
        {
            tempData[typeof(T).FullName] = value;
        }
    }
}
#endif