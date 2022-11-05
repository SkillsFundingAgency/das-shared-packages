using System;

namespace SFA.DAS.DfESignIn.SampleSite.Classic.Api.Models
{
    public class Organisation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Urn { get; set; }
        public string Uid { get; set; }
        public int? UkPrn { get; set; }
        public string LegacyId { get; set; }
        public string Sid { get; set; }
        public string DistrictAdministrative_Code { get; set; }
    }
}
