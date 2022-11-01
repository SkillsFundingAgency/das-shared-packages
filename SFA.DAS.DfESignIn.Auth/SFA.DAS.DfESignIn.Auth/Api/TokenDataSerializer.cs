﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace SFA.DAS.DfESignIn.Auth.Api
{
    public sealed class TokenDataSerializer : ITokenDataSerializer
    {
        private readonly JsonSerializer _serializer;

        public TokenDataSerializer() : this(JsonSerializer.CreateDefault()) { }

        public TokenDataSerializer(JsonSerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }
            _serializer = serializer;
        }

        public string Serialize(object obj) =>
            JObject.FromObject(obj, _serializer).ToString(_serializer.Formatting, _serializer.Converters.ToArray());
    }
}