using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        public GovUkCoreIdentityJwt CoreIdentityJwt { get; set; }

        [JsonPropertyName("https://vocab.account.gov.uk/v1/address")]
        public List<GovUkAddress> Addresses { get; set; }

        [JsonPropertyName("https://vocab.account.gov.uk/v1/drivingPermit")]
        public List<GovUkDrivingPermit> DrivingPermits { get; set; }

        [JsonPropertyName("https://vocab.account.gov.uk/v1/passport")]
        public List<GovUkPassport> Passports { get; set; }

        [JsonPropertyName("https://vocab.account.gov.uk/v1/returnCode")]
        public List<GovUkReturnCode> ReturnCodes { get; set; }
    }

    [JsonConverter(typeof(CoreIdentityJwtConverter))]
    public class GovUkCoreIdentityJwt
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; }

        [JsonPropertyName("vot")]
        public string Vot { get; set; }

        [JsonPropertyName("vc")]
        public GovUkCoreIdentityCredential Vc { get; set; }
    }

    public class GovUkCoreIdentityCredential
    {
        [JsonPropertyName("credentialSubject")]
        public GovUkCredentialSubject CredentialSubject { get; set; }
    }

    public class GovUkCredentialSubject
    {
        [JsonPropertyName("birthDate")]
        public List<GovUkBirthDateEntry> BirthDates { get; set; }

        [JsonPropertyName("name")]
        public List<GovUkName> Names { get; set; }

        public List<GovUkHistoricalName> GetHistoricalNames()
        {
            var normalizedParts = new List<(DateTime From, DateTime Until, string Type, string Value)>();

            // normalize name parts
            foreach (var name in Names)
            {
                var nameFrom = name.ValidFrom ?? DateTime.MinValue;
                var nameUntil = name.ValidUntil ?? DateTime.MaxValue;

                foreach (var part in name.NameParts ?? Enumerable.Empty<GovUkNamePart>())
                {
                    var partFrom = part.ValidFrom ?? nameFrom;
                    var partUntil = part.ValidUntil ?? nameUntil;

                    normalizedParts.Add((partFrom, partUntil, part.Type, part.Value));
                }
            }

            if (normalizedParts.Count == 0)
                return new List<GovUkHistoricalName>();

            // determine change points
            var changePoints = new HashSet<DateTime>();
            foreach (var part in normalizedParts)
            {
                changePoints.Add(part.From);
                changePoints.Add(part.Until);
            }

            var orderedChangePoints = changePoints.OrderBy(d => d).ToList();
            
            // build timeline slices
            var slices = new List<GovUkHistoricalName>();
            for (int i = 0; i < orderedChangePoints.Count - 1; i++)
            {
                var sliceFrom = orderedChangePoints[i];
                var sliceUntil = orderedChangePoints[i + 1];

                var activeParts = normalizedParts
                    .Where(p => p.From < sliceUntil && p.Until > sliceFrom)
                    .ToList();

                var givenNames = activeParts
                    .Where(p => p.Type == "GivenName")
                    .Select(p => p.Value)
                    .ToList();

                var familyNames = activeParts
                    .Where(p => p.Type == "FamilyName")
                    .Select(p => p.Value)
                    .ToList();

                if (givenNames.Count == 0 && familyNames.Count == 0)
                    continue;

                slices.Add(new GovUkHistoricalName
                {
                    GivenNames = string.Join(" ", givenNames),
                    FamilyNames = string.Join(" ", familyNames),
                    ValidFrom = sliceFrom,
                    ValidUntil = sliceUntil
                });
            }

            // collapse adjacent identical names
            var collapsed = new List<GovUkHistoricalName>();
            foreach (var slice in slices)
            {
                if (collapsed.Count == 0)
                {
                    collapsed.Add(slice);
                    continue;
                }

                var last = collapsed[^1];
                if (last.GivenNames == slice.GivenNames && last.FamilyNames == slice.FamilyNames)
                {
                    last.ValidUntil = slice.ValidUntil;
                }
                else
                {
                    collapsed.Add(slice);
                }
            }

            return collapsed;
        }
    }

    public class GovUkBirthDateEntry
    {
        [JsonPropertyName("validFrom")]
        public string ValidFromRaw { get; set; }

        [JsonPropertyName("validUntil")]
        public string ValidUntilRaw { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonIgnore]
        public DateTime? ValidFrom => DateTime.TryParse(ValidFromRaw, CultureInfo.InvariantCulture, out var dt) ? dt : null;

        [JsonIgnore]
        public DateTime? ValidUntil => DateTime.TryParse(ValidUntilRaw, CultureInfo.InvariantCulture, out var dt) ? dt : null;
    }

    public class GovUkName
    {
        [JsonPropertyName("validFrom")]
        public string ValidFromRaw { get; set; }

        [JsonPropertyName("validUntil")]
        public string ValidUntilRaw { get; set; }

        [JsonIgnore]
        public DateTime? ValidFrom => DateTime.TryParse(ValidFromRaw, CultureInfo.InvariantCulture, out var dt) ? dt : null;

        [JsonIgnore]
        public DateTime? ValidUntil => DateTime.TryParse(ValidUntilRaw, CultureInfo.InvariantCulture, out var dt) ? dt : null;

        [JsonPropertyName("nameParts")]
        public List<GovUkNamePart> NameParts { get; set; }
    }


    public class GovUkNamePart
    {
        [JsonPropertyName("validFrom")]
        public string ValidFromRaw { get; set; }

        [JsonPropertyName("validUntil")]
        public string ValidUntilRaw { get; set; }

        [JsonIgnore]
        public DateTime? ValidFrom => DateTime.TryParse(ValidFromRaw, CultureInfo.InvariantCulture, out var dt) ? dt : null;

        [JsonIgnore]
        public DateTime? ValidUntil => DateTime.TryParse(ValidUntilRaw, CultureInfo.InvariantCulture, out var dt) ? dt : null;

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

    }

    public class GovUkHistoricalName
    {
        public string GivenNames { get; set; }
        public string FamilyNames { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
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

    public class CoreIdentityJwtConverter : JsonConverter<GovUkCoreIdentityJwt>
    {
        public override GovUkCoreIdentityJwt Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string json = string.Empty;

            if (reader.TokenType == JsonTokenType.String)
            {
                var jwt = reader.GetString();
                if (string.IsNullOrEmpty(jwt)) return null;

                var parts = jwt.Split('.');
                if (parts.Length != 3) return null;

                try
                {
                    json = DecodeBase64Url(parts[1]);
                }
                catch
                {
                    return null;
                }
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                json = doc.RootElement.GetRawText();
            }
            else
            {
                throw new JsonException($"Unexpected token type {reader.TokenType} for CoreIdentityJwt");
            }

            return DeserializeInternal(json, options);
        }

        public override void Write(Utf8JsonWriter writer, GovUkCoreIdentityJwt value, JsonSerializerOptions options)
        {
            var internalObject = new GovUkCoreIdentityJwtInternal
            {
                Sub = value.Sub,
                Vot = value.Vot,
                Vc = value.Vc
            };

            JsonSerializer.Serialize(writer, internalObject, options);
        }

        /// <summary>
        /// Serialize CoreIdentityJwt into a fake JWT string. Don’t need a header or signature, just encode the payload as 
        /// base64url and wrap it as header.payload.signature format.
        /// </summary>
        /// <remarks>
        /// The signature is empty, which is legal for alg=none
        /// </remarks>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static string SerializeStubCoreIdentityJwt(GovUkCoreIdentityJwt payload)
        {
            var header = new { alg = "none", typ = "JWT" };
            string Encode(string json)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                return Convert.ToBase64String(bytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }

            var encodedHeader = Encode(JsonSerializer.Serialize(header));
            var encodedPayload = Encode(SerializeInternal(payload));

            return $"{encodedHeader}.{encodedPayload}.";
        }

        private static string SerializeInternal(GovUkCoreIdentityJwt govUkCoreIdentityJwt)
        {
            var internalObject = new GovUkCoreIdentityJwtInternal
            { 
                Sub = govUkCoreIdentityJwt.Sub,
                Vot = govUkCoreIdentityJwt.Vot,
                Vc = govUkCoreIdentityJwt.Vc
            };

            return JsonSerializer.Serialize(internalObject);
        }

        private static GovUkCoreIdentityJwt DeserializeInternal(string json, JsonSerializerOptions options)
        {
            var internalObj = JsonSerializer.Deserialize<GovUkCoreIdentityJwtInternal>(json, options);
            return new GovUkCoreIdentityJwt
            {
                Sub = internalObj.Sub,
                Vot = internalObj.Vot,
                Vc = internalObj.Vc
            };
        }

        private static string DecodeBase64Url(string input)
        {
            var pad = 4 - (input.Length % 4);
            if (pad < 4) input += new string('=', pad);

            var bytes = Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/'));
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        private sealed class GovUkCoreIdentityJwtInternal
        {
            [JsonPropertyName("sub")]
            public string Sub { get; set; }

            [JsonPropertyName("vot")]
            public string Vot { get; set; }

            [JsonPropertyName("vc")]
            public GovUkCoreIdentityCredential Vc { get; set; }
        }
    }
}