using System.Collections.Generic;
using SFA.DAS.DfESignIn.Auth.Configuration;

namespace SFA.DAS.DfESignIn.Auth.Models.Stub
{
    public class SignInStubViewModel
    {
        public string UserId { get; set; }

        public string ReturnUrl { get; set; }

        public List<DfESignInStubUser> Users { get; set; } =
            new List<DfESignInStubUser>();
    }
}