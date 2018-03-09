using System.Web;

namespace SFA.DAS.Web.Policy
{
   
    public class ResponseHeaderRestrictedCachePolicy : IHttpResponsePolicy
    {

        public void Apply(HttpContextBase context)
        {
            context?.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context?.Response.Cache.AppendCacheExtension("no-store, must-revalidate");
            context?.Response.Headers.Add("Pragma", "no-cache");
            context?.Response.Headers.Add("Expires", "0");
        }

        public PolicyConcern Concerns { get; } = PolicyConcern.HttpResponse;
    }
}