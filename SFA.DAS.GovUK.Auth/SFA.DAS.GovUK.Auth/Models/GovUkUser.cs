using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SFA.DAS.GovUK.Auth.Models
{
    public class GovUkUser
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("phone_number_verified")]
        public bool PhoneNumberVerified { get; set; }

        [JsonPropertyName("https://vocab.account.gov.uk/v1/coreIdentityJWT")]
        public string CoreIdentityJwt { get; set; }

        [JsonPropertyName("https://vocab.account.gov.uk/v1/address")]
        public List<GovUkAddress> Address { get; set; }

        [JsonPropertyName("https://vocab.account.gov.uk/v1/drivingPermit")]
        public List<GovUkDrivingPermit> DrivingPermit { get; set; }

        [JsonPropertyName("https://vocab.account.gov.uk/v1/passport")]
        public List<GovUkPassport> Passport { get; set; }

        [JsonPropertyName("https://vocab.account.gov.uk/v1/returnCode")]
        public List<GovUkReturnCode> ReturnCodes { get; set; }
    }

    public class GovUkAddress
    {
        public string Uprn { get; set; }
        public string OrganisationName { get; set; }
        public string DepartmentName { get; set; }
        public string SubBuildingName { get; set; }
        public string BuildingNumber { get; set; }
        public string BuildingName { get; set; }
        public string DependentStreetName { get; set; }
        public string StreetName { get; set; }
        public string DoubleDependentAddressLocality { get; set; }
        public string DependentAddressLocality { get; set; }
        public string AddressLocality { get; set; }
        public string AddressRegion { get; set; }
        public string PostalCode { get; set; }
        public string AddressCountry { get; set; }

        public DateOnly? ValidFrom { get; set; }
        public DateOnly? ValidUntil { get; set; }
    }

    public class GovUkDrivingPermit
    {
        public string ExpiryDate { get; set; }
        public string IssueNumber { get; set; }
        public string IssuedBy { get; set; }
        public string PersonalNumber { get; set; }
    }

    public class GovUkPassport
    {
        public string DocumentNumber { get; set; }
        public string IcaoIssuerCode { get; set; }
        public string ExpiryDate { get; set; }
    }

    public class GovUkReturnCode
    {
        public string Code { get; set; }
    }
}