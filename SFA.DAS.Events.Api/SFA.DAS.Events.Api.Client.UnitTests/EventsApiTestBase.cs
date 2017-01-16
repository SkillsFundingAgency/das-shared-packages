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
        protected Mock<IEventsApiClientConfiguration> Config;
        protected Mock<ISecureHttpClient> SecureHttpClient;

        [SetUp]
        public void Arrange()
        {
            Config = new Mock<IEventsApiClientConfiguration>();
            Config.SetupProperty(x => x.ClientToken, ClientToken);
            Config.SetupProperty(x => x.BaseUrl, BaseUrl);

            SecureHttpClient = new Mock<ISecureHttpClient>();

            Api = new EventsApi(SecureHttpClient.Object, Config.Object);
        }
    }
}
