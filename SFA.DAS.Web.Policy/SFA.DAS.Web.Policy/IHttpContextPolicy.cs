using System.Web;

namespace SFA.DAS.Web.Policy
{
    public interface IHttpContextPolicy
    {
        PolicyConcern Concerns { get; } 
        void Apply(HttpContextBase context);
    }
}