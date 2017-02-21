using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.Events.Api.Client.Configuration;

namespace SFA.DAS.Events.Api.Client.UnitTests
{
    public abstract class EventsApiTestBase
    {
        protected const string ClientToken = "ASDFJL40|";
        protected const string BaseUrl = "http://test.com/";

        protected EventsApi Api;
        protected ClientConfiguration Config;
        protected Mock<ISecureHttpClient> SecureHttpClient;

        [SetUp]
        public void Arrange()
        {
            Config = new ClientConfiguration();
            Config.ClientToken = ClientToken;
            Config.BaseUrl = BaseUrl;

            SecureHttpClient = new Mock<ISecureHttpClient>();

            Api = new EventsApi(SecureHttpClient.Object, Config);
        }

        public class ClientConfiguration : IEventsApiClientConfiguration
        {
            public string BaseUrl { get; set; }
            public string ClientToken { get; set; }
        }
    }
}
