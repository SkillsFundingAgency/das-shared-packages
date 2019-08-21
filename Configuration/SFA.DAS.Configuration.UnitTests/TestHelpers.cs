using Newtonsoft.Json;

namespace SFA.DAS.Configuration.UnitTests
{
    public static class TestHelpers
    {
        public static T Clone<T>(this T source)
        {
            if (ReferenceEquals(source, null)) return default;

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }
}