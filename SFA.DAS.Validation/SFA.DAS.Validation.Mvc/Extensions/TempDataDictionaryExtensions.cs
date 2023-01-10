using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace SFA.DAS.Validation.Mvc.Extensions
{
    public static class TempDataDictionaryExtensions
    {
        public static T Get<T>(this ITempDataDictionary tempData) where T : class
        {
            return tempData.TryGetValue(typeof(T).FullName, out var value) ? JsonConvert.DeserializeObject<T>((string)value) : null;
        }

        public static void Set<T>(this ITempDataDictionary tempData, T value) where T : class
        {
            tempData[typeof(T).FullName] = JsonConvert.SerializeObject(value);
        }
    }
}
