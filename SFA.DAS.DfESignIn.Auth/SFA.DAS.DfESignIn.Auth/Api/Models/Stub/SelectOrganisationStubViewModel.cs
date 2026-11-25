using System;
using System.Collections.Generic;
using SFA.DAS.DfESignIn.Auth.Configuration;

namespace SFA.DAS.DfESignIn.Auth.Models.Stub
{
    public class SelectOrganisationStubViewModel
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        public Guid? OrganisationId { get; set; }

        public string ReturnUrl { get; set; }

        public List<DfESignInStubOrganisation> Organisations { get; set; } =
            new List<DfESignInStubOrganisation>();
    }
}