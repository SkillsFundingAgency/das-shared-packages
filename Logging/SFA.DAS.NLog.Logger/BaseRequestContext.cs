namespace SFA.DAS.NLog.Logger
{
    public class BaseRequestContext : IRequestContext
    {
        public BaseRequestContext(string url, string ipAddress, string applicationName)
        {
            Url = url;
            IpAddress = ipAddress;
            ApplicationName = applicationName;
        }

        public string Url { get; }

        public string IpAddress { get; }

        public string ApplicationName { get; }
    }
}