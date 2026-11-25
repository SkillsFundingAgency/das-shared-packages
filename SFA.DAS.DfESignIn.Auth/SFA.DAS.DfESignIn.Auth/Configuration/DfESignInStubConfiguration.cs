using System.Collections.Generic;

namespace SFA.DAS.DfESignIn.Auth.Configuration
{
    public class DfESignInStubConfiguration
    {
        public string DefaultUserId { get; set; }

        public List<DfESignInStubUser> Users { get; set; } =
            new List<DfESignInStubUser>();
    }
}