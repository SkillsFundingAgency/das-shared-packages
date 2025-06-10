using System;
using System.Collections.Generic;
using System.Text.Json;
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
        [JsonPropertyName("uprn")]
        [JsonConverter(typeof(UprnConverter))]
        public string Uprn { get; set; }

        [JsonPropertyName("organisationName")]
        public string OrganisationName { get; set; }

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("subBuildingName")]
        public string SubBuildingName { get; set; }

        [JsonPropertyName("buildingNumber")]
        public string BuildingNumber { get; set; }

        [JsonPropertyName("buildingName")]
        public string BuildingName { get; set; }

        [JsonPropertyName("dependentStreetName")]
        public string DependentStreetName { get; set; }

        [JsonPropertyName("streetName")]
        public string StreetName { get; set; }

        [JsonPropertyName("doubleDependentAddressLocality")]
        public string DoubleDependentAddressLocality { get; set; }

        [JsonPropertyName("dependentAddressLocality")]
        public string DependentAddressLocality { get; set; }

        [JsonPropertyName("addressLocality")]
        public string AddressLocality { get; set; }

        [JsonPropertyName("addressRegion")]
        public string AddressRegion { get; set; }

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("addressCountry")]
        public string AddressCountry { get; set; }

        [JsonPropertyName("validFrom")]
        public DateOnly? ValidFrom { get; set; }

        [JsonPropertyName("validUntil")]
        public DateOnly? ValidUntil { get; set; }
    }

    public class GovUkDrivingPermit
    {
        [JsonPropertyName("expiryDate")]
        public DateOnly? ExpiryDate { get; set; }

        [JsonPropertyName("issueNumber")]
        public string IssueNumber { get; set; }

        [JsonPropertyName("issuedBy")]
        public string IssuedBy { get; set; }

        [JsonPropertyName("personalNumber")]
        public string PersonalNumber { get; set; }
    }
    public class GovUkPassport
    {
        [JsonPropertyName("documentNumber")]
        public string DocumentNumber { get; set; }

        [JsonPropertyName("icaoIssuerCode")]
        public string IcaoIssuerCode { get; set; }

        [JsonPropertyName("expiryDate")]
        public DateOnly? ExpiryDate { get; set; }
    }

    public class GovUkReturnCode
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }

    public class UprnConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetDouble().ToString("0"),
                JsonTokenType.String => reader.GetString(),
                _ => throw new JsonException("Invalid type for UPRN")
            };
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}