using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public sealed class TokenDataSerializer(JsonSerializer serializer) : ITokenDataSerializer
    {
        private readonly JsonSerializer _serializer = serializer ?? throw new ArgumentNullException("serializer");

        public TokenDataSerializer() : this(JsonSerializer.CreateDefault()) { }

        public string Serialize(object obj) =>
            JObject.FromObject(obj, _serializer).ToString(_serializer.Formatting, _serializer.Converters.ToArray());
    }

    public interface ITokenDataSerializer
    {
        string Serialize(object obj);
    }
}
