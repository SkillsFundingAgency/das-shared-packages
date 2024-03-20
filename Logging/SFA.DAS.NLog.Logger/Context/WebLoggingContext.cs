using System.Web;

namespace SFA.DAS.NLog.Logger
{
    public sealed class WebLoggingContext : IWebLoggingContext
    {
        public WebLoggingContext(HttpContextBase context)
        {
            IpAddress = context?.Request.UserHostAddress;
            Url = context?.Request.RawUrl;
        }

        public string IpAddress { get; private set; }

        public string Url { get; private set; }
    }
}