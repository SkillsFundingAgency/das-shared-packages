using System.Collections.Generic;

namespace SFA.DAS.DfESignIn.Auth.Configuration
{
    public class DfESignInStubUser
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string GivenName { get; set; } = string.Empty;

        public string FamilyName { get; set; } = string.Empty;

        public List<DfESignInStubOrganisation> Organisations { get; set; }
            = new List<DfESignInStubOrganisation>();
    }
}