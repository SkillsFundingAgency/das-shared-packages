using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.MessageHandlers;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.Http
{
    public static class IHttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddLogging(this IHttpClientBuilder httpBuilder)
        {
            httpBuilder.AddHttpMessageHandler(sp =>
            {
                var loggerFactory = sp.GetService<ILoggerFactory>();
                return new LoggingMessageHandler(loggerFactory.CreateLogger<LoggingMessageHandler>());
            });

            return httpBuilder;
        }

        public static IHttpClientBuilder AddApimAuthorisationHeader(this IHttpClientBuilder httpBuilder, IApimClientConfiguration config)
        {
            httpBuilder.AddHttpMessageHandler(sp => new ApimHeadersHandler(config));
            return httpBuilder;
        }

        public static IHttpClientBuilder AddDefaultHeaders(this IHttpClientBuilder httpBuilder)
        {
            httpBuilder.AddHttpMessageHandler(sp => new DefaultHeadersHandler());
            return httpBuilder;
        }

        public static IHttpClientBuilder AddManagedIdentityAuthorisationHeader(this IHttpClientBuilder httpBuilder, IManagedIdentityTokenGenerator tokenGenerator)
        {
            httpBuilder.AddHttpMessageHandler(sp => new ManagedIdentityHeadersHandler(tokenGenerator));
            return httpBuilder;
        }
    }
}
