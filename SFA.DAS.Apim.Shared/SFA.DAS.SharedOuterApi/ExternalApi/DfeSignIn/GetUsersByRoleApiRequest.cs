using System;
using SFA.DAS.SharedOuterApi.Interfaces;

namespace SFA.DAS.SharedOuterApi.ExternalApi.DfeSignIn
{
    public class GetUsersByRoleApiRequest : IGetApiRequest
    {
        private readonly string _ukprn;
        private readonly string _role;

        public GetUsersByRoleApiRequest(string ukprn, string role)
        {
            _ukprn = ukprn ?? throw new ArgumentNullException(nameof(ukprn));
            _role = role ?? throw new ArgumentNullException(nameof(role));
        }

        public string GetUrl => 
            $"organisations/{Uri.EscapeDataString(_ukprn)}/users?roles={Uri.EscapeDataString(_role)}";

    }
}
