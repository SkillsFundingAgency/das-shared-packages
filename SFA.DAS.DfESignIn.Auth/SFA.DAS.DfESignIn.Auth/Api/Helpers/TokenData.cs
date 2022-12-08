﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public class TokenData : ITokenData, IDisposable
    {
        public IDictionary<string, object> Header { get; set; }

        public IDictionary<string, object> Payload { get; set; }

        public TokenData() : this(null, null) { }

        public TokenData(IDictionary<string, object> header, IDictionary<string, object> payload)
        {
            Header = header ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            Payload = payload ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        protected virtual void Dispose(bool disposing)
        {
            Header.Clear();
            Payload.Clear();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public interface ITokenData
    {
        IDictionary<string, object> Header { get; set; }
        IDictionary<string, object> Payload { get; set; }
        void Dispose();
    }
}
