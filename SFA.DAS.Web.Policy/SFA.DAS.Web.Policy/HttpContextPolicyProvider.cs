using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.Web.Policy
{
    public class HttpContextPolicyProvider
    {
        private readonly IEnumerable<IHttpContextPolicy> _policies;

        public HttpContextPolicyProvider(IEnumerable<IHttpContextPolicy> policies)
        {
            _policies = policies;
        }
        public void Apply(HttpContextBase context, PolicyConcern concern)
        {
            foreach (var httpContextPolicyAgent in _policies.Where(x=>x.Concerns == concern))
            {
                httpContextPolicyAgent.Apply(context);
            }
        }
    }
}
