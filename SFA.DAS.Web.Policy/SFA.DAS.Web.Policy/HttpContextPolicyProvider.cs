using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.Web.Policy
{
    public class HttpContextPolicyProvider
    {
        private readonly IEnumerable<IHttpContextPolicy> _policies;

        /// <summary>
        /// Allows a custom policy implementation
        /// </summary>
        /// <param name="policies"></param>
        public HttpContextPolicyProvider(IEnumerable<IHttpContextPolicy> policies)
        {
            _policies = policies;
        }
        /// <summary>
        /// Default operation is to provide all implemented policies. Use explicit constructor to select custom policy implementation
        /// </summary>
        public HttpContextPolicyProvider()
        {
            _policies = new List<IHttpContextPolicy>()
            {
                new ResponseHeaderRestrictionPolicy(),
                new ResponseHeaderRestrictedCachePolicy(),
                new ResponseHeaderXOptionsPolicy()
            };
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
