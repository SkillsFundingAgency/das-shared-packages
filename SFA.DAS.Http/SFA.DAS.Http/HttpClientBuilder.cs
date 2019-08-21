using SFA.DAS.Http.MessageHandlers;
using SFA.DAS.Http.TokenGenerators;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Http
{
    public sealed class HttpClientBuilder
    {
        private DelegatingHandler _rootHandler;
        private DelegatingHandler _leafHandler;

        public HttpClient Build()
        {
            if (_rootHandler == null)
                return new HttpClient();

            _leafHandler.InnerHandler = new HttpClientHandler();

            return new HttpClient(_rootHandler);
        }

        public HttpClientBuilder WithDefaultHeaders()
        {
            var newHandler = new DefaultHeadersHandler();

            AddHandlerToChain(newHandler);

            return this;
        }


        public HttpClientBuilder WithHandler(DelegatingHandler handler)
        {
            AddHandlerToChain(handler);

            return this;
        }

        public HttpClientBuilder WithBearerAuthorisationHeader(IGenerateBearerToken tokenGenerator)
        {
            var newHandler = new SecurityMessageHandler(tokenGenerator);

            AddHandlerToChain(newHandler);

            return this;
        }

        public HttpClientBuilder WithLogging(ILoggerFactory loggerFactory)
        {
            var loggingMessageHandler = new LoggingMessageHandler(loggerFactory.CreateLogger<LoggingMessageHandler>());

            AddHandlerToChain(loggingMessageHandler);
            
            return this;
        }

        public static implicit operator HttpClient(HttpClientBuilder handler)
        {
            return handler?.Build();
        }

        private void AddHandlerToChain(DelegatingHandler newHandler)
        {
            if (_rootHandler == null)
            {
                _rootHandler = newHandler;
                _leafHandler = newHandler;
            }
            else
            {
                _leafHandler.InnerHandler = newHandler;
                _leafHandler = newHandler;
            }
        }
    }
}
