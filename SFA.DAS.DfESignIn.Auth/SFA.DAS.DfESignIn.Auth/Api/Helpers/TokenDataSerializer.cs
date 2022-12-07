using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public sealed class TokenDataSerializer : ITokenDataSerializer
    {
        private readonly JsonSerializer _serializer;

        public TokenDataSerializer() : this(JsonSerializer.CreateDefault()) { }

        public TokenDataSerializer(JsonSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException("serializer");
        }

        public string Serialize(object obj) =>
            JObject.FromObject(obj, _serializer).ToString(_serializer.Formatting, _serializer.Converters.ToArray());
    }

    public interface ITokenDataSerializer
    {
        string Serialize(object obj);
    }
}
