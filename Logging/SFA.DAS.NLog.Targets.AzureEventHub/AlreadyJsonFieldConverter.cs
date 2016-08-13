using System;
using Newtonsoft.Json;

namespace SFA.DAS.NLog.Targets.AzureEventHub
{
    public sealed class AlreadyJsonFieldConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is string)
            {
                var stringValue = value as string;

                var newValue = stringValue.Replace('"', '\''); ;
                writer.WriteToken(JsonToken.String, newValue);
            }
            else
            {
                writer.WriteToken(JsonToken.Raw, value);
            }
        }
    }


}
