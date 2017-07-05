using Newtonsoft.Json;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client
{
    internal static class GenericEventMapper
    {
        public static GenericEvent FromTyped<T>(IGenericEvent<T> source)
        {
            return new GenericEvent
            {
                Id = source.Id,
                Type = source.Type,
                CreatedOn = source.CreatedOn,
                Payload =
                    JsonConvert.SerializeObject(source.Payload,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}),
                ResourceId = source.ResourceId,
                ResourceType = source.ResourceType
            };
        }

        public static IGenericEvent<T> ToTyped<T>(GenericEvent source)
        {
            return new GenericEvent<T>
            {
                Payload = JsonConvert.DeserializeObject<T>(source.Payload),
                Id = source.Id,
                CreatedOn = source.CreatedOn,
                ResourceId = source.ResourceId,
                ResourceType = source.ResourceType
            };
        }
    }
}