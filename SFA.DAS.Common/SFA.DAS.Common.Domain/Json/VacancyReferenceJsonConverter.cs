using Newtonsoft.Json;
using SFA.DAS.Common.Domain.Models;
using System;

namespace SFA.DAS.Common.Domain.Json
{
    public class VacancyReferenceJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(VacancyReference);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return VacancyReference.None;
                case JsonToken.String:
                    return new VacancyReference((string)reader.Value);
                case JsonToken.Integer:
                    return new VacancyReference((long)(reader.Value ?? 0));
                case JsonToken.None:
                case JsonToken.StartObject:
                case JsonToken.StartArray:
                case JsonToken.StartConstructor:
                case JsonToken.PropertyName:
                case JsonToken.Comment:
                case JsonToken.Raw:
                case JsonToken.Float:
                case JsonToken.Boolean:
                case JsonToken.Undefined:
                case JsonToken.EndObject:
                case JsonToken.EndArray:
                case JsonToken.EndConstructor:
                case JsonToken.Date:
                case JsonToken.Bytes:
                default:
                    return VacancyReference.None;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                var reference = (VacancyReference)value;
                if (VacancyReference.None.Equals(reference))
                    writer.WriteNull();
                else
                    writer.WriteValue(reference.ToString());
            }
        }
    }
}
