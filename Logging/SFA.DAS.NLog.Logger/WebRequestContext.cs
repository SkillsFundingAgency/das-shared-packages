using System.Web;

namespace SFA.DAS.NLog.Logger
{
    public class WebRequestContext : BaseRequestContext
    {
        public WebRequestContext(HttpContextBase context, string applicationName) 
            : base(context?.Request.RawUrl, context?.Request.UserHostAddress, applicationName) { }
    }
}