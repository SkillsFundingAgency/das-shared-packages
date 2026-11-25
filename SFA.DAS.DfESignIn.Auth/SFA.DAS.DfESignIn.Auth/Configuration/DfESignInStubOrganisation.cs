using System;
using System.Collections.Generic;
using SFA.DAS.DfESignIn.Auth.Api.Models;

namespace SFA.DAS.DfESignIn.Auth.Configuration
{
    public class DfESignInStubOrganisation
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int? UkPrn { get; set; }

        public List<Role> Roles { get; set; } = new List<Role>();
    }
}