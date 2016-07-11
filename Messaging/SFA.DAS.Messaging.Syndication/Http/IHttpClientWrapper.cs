using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Syndication.Http
{
    public interface IHttpClientWrapper : IDisposable
    {
        Task<string> Get(string resourceUri, IDictionary<string, string[]> headers = null);
        void Dispose();
    }
}